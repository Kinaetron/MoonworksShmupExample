using System.Numerics;
using System.Runtime.InteropServices;
using MoonWorks.Storage;
using MoonWorks.Graphics;
using MoonWorks;
using Buffer = MoonWorks.Graphics.Buffer;

namespace MoonWorksShumpExample.Graphics;

public class SpriteBatch
{
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    struct PositionTextureColorVertex : IVertexType
    {
        [FieldOffset(0)]
        public Vector4 Position;

        [FieldOffset(16)]
        public Vector2 TexCoord;

        [FieldOffset(32)]
        public Vector4 Color;

        public static VertexElementFormat[] Formats { get; } =
        [
        VertexElementFormat.Float4,
        VertexElementFormat.Float2,
        VertexElementFormat.Float4
        ];

        public static uint[] Offsets { get; } =
        [
        0,
        16,
        32
        ];
    }

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    struct ComputeSpriteData
    {
        [FieldOffset(0)]
        public Vector3 Position;

        [FieldOffset(12)]
        public float Rotation;

        [FieldOffset(16)]
        public Vector2 Size;

        [FieldOffset(32)]
        public Vector4 Color;
    }

    private readonly Texture _renderTarget;
    private readonly Matrix4x4 _worldSpace;

    private Color _clearColor;
    private readonly Window _window;
    private readonly TitleStorage _titleStorage;
    private readonly GraphicsDevice _graphicsDevice;

    private readonly Sampler _sampler;
    private readonly ComputePipeline _computePipeline;
    private readonly GraphicsPipeline _renderPipeline;

    private readonly TransferBuffer _spriteComputeTransferBuffer;
    private readonly Buffer _spriteComputeBuffer;
    private readonly Buffer _spriteVertexBuffer;
    private readonly Buffer _spriteIndexBuffer;

    private const int _maxSpriteCount = 8192;

    private Dictionary<Texture, List<ComputeSpriteData>> _spriteData = [];

    private Matrix4x4 _batchMatrix = Matrix4x4.Identity;

    private readonly ResourceUploader _resourceUploader;

    private bool _batcher;

    public SpriteBatch(
        uint renderTargetX,
        uint renderTargetY,
        GraphicsDevice graphicsDevice, 
        TitleStorage titleStorage,
        Window window)
    {
        _window = window;
        _titleStorage = titleStorage;
        _graphicsDevice = graphicsDevice;

        var vertexShader = ShaderCross.Create(
            _graphicsDevice,
            titleStorage,
            "Content/Shaders/TexturedQuadColorWithMatrix.vert.hlsl",
            "main",
            ShaderCross.ShaderFormat.HLSL,
            ShaderStage.Vertex);

        var fragmentShader = ShaderCross.Create(
            _graphicsDevice,
            titleStorage,
            "Content/Shaders/TexturedQuadColor.frag.hlsl",
            "main",
            ShaderCross.ShaderFormat.HLSL,
            ShaderStage.Fragment);

            var renderPipelineCreateInfo = new GraphicsPipelineCreateInfo
            {
                TargetInfo = new GraphicsPipelineTargetInfo
                {
                    ColorTargetDescriptions = 
                    [
                        new ColorTargetDescription
                        {
                            Format = _window.SwapchainFormat,
                            BlendState = ColorTargetBlendState.PremultipliedAlphaBlend
                        }
                    ]
                },
                DepthStencilState = DepthStencilState.Disable,
                MultisampleState = MultisampleState.None,
                PrimitiveType = PrimitiveType.TriangleList,
                RasterizerState = RasterizerState.CCW_CullNone,
                VertexInputState = VertexInputState.Empty,
                VertexShader = vertexShader,
                FragmentShader = fragmentShader
            };

            renderPipelineCreateInfo.VertexInputState = VertexInputState.CreateSingleBinding<PositionTextureColorVertex>();
            _renderPipeline = GraphicsPipeline.Create(_graphicsDevice, renderPipelineCreateInfo);

            _computePipeline = ShaderCross.Create(
                _graphicsDevice,
                titleStorage,
                "Content/Shaders/SpriteBatch.comp.hlsl",
                "main",
                ShaderCross.ShaderFormat.HLSL);

        _sampler = Sampler.Create(_graphicsDevice, SamplerCreateInfo.PointClamp);

        _spriteComputeTransferBuffer = TransferBuffer.Create<ComputeSpriteData>(
            _graphicsDevice,
            TransferBufferUsage.Upload,
            _maxSpriteCount);

        _spriteComputeBuffer = Buffer.Create<ComputeSpriteData>(
            _graphicsDevice,
            BufferUsageFlags.ComputeStorageRead,
            _maxSpriteCount);

        _spriteVertexBuffer = Buffer.Create<PositionTextureColorVertex>(
            _graphicsDevice,
            BufferUsageFlags.ComputeStorageWrite | BufferUsageFlags.Vertex,
            _maxSpriteCount * 4);

        _spriteIndexBuffer = Buffer.Create<uint>(
            _graphicsDevice,
            BufferUsageFlags.Index,
            _maxSpriteCount * 6);

        var spriteIndexTransferBuffer = TransferBuffer.Create<uint>(
            _graphicsDevice,
            TransferBufferUsage.Upload,
            _maxSpriteCount * 6);

        var indexSpan = spriteIndexTransferBuffer.Map<uint>(false);

        for (int i = 0, j = 0; i < _maxSpriteCount * 6; i += 6, j += 4)
        {
            indexSpan[i]     =  (uint)j;
            indexSpan[i + 1] =  (uint)j + 1;
            indexSpan[i + 2] =  (uint)j + 2;
            indexSpan[i + 3] =  (uint)j + 3;
            indexSpan[i + 4] =  (uint)j + 2;
            indexSpan[i + 5] =  (uint)j + 1;
        }
        spriteIndexTransferBuffer.Unmap();

        var commandBuffer = _graphicsDevice.AcquireCommandBuffer();
        var copyPass = commandBuffer.BeginCopyPass();
        copyPass.UploadToBuffer(spriteIndexTransferBuffer, _spriteIndexBuffer, false);
        commandBuffer.EndCopyPass(copyPass);
        _graphicsDevice.Submit(commandBuffer);

        _worldSpace = Matrix4x4.CreateOrthographicOffCenter(
           0,
           renderTargetX,
           renderTargetY,
           0,
           0,
           -1f);

        _renderTarget =
            Texture.Create2D(
                _graphicsDevice,
                renderTargetX,
                renderTargetY,
                TextureFormat.B8G8R8A8Unorm,
                TextureUsageFlags.ColorTarget | TextureUsageFlags.Sampler);


        _resourceUploader = new ResourceUploader(_graphicsDevice);
    }

    public Texture CreateTexture(string filePath)
    {
        var texture = _resourceUploader.CreateTexture2DFromCompressed(
            _titleStorage,
            filePath,
            TextureFormat.R8G8B8A8Unorm,
            TextureUsageFlags.Sampler);

        _resourceUploader.Upload();

        _spriteData.Add(texture, []);

        return texture;
    }

    public void Begin(Color clearColor, Matrix4x4 matrix)
    {
        _batcher = true;
        _batchMatrix = matrix;
        _clearColor = clearColor;
    }

    public void Draw(Texture texture, Vector2 position, float rotation, Vector2 size, Color color)
    {
        if(!_batcher)
        {
            throw new Exception("You must call Begin Method before you call Draw");
        }

        if(_spriteData.Count > _maxSpriteCount) {
            End();
        }

        var data = new ComputeSpriteData
        {
            Position = new Vector3(position.X, position.Y, 0),
            Rotation = rotation,
            Size = size,
            Color = color.ToVector4()
        };

        if (_spriteData.TryGetValue(texture, out List<ComputeSpriteData>? value))
        {
            value.Add(data);
        }
    }

    public void End()
    {
        if (!_batcher)
        {
            throw new Exception("You must call Begin Method before you call Draw");
        }

        var commandBuffer = _graphicsDevice.AcquireCommandBuffer();
        var swapchainTexture = commandBuffer.AcquireSwapchainTexture(_window);

        if (swapchainTexture != null)
        {
            int spriteOffset = 0;
            foreach (var spriteData in _spriteData)
            {
                var data = _spriteComputeTransferBuffer.Map<ComputeSpriteData>(true);
                for (int i = 0; i < spriteData.Value.Count; i++)
                {
                    data[spriteOffset + i].Position = spriteData.Value[i].Position;
                    data[spriteOffset + i].Rotation = spriteData.Value[i].Rotation;
                    data[spriteOffset + i].Size = spriteData.Value[i].Size;
                    data[spriteOffset + i].Color = spriteData.Value[i].Color;
                }
                spriteOffset += spriteData.Value.Count;
            }
            _spriteComputeTransferBuffer.Unmap();

            var copyPass = commandBuffer.BeginCopyPass();
            copyPass.UploadToBuffer(_spriteComputeTransferBuffer, _spriteComputeBuffer, true);
            commandBuffer.EndCopyPass(copyPass);

            var computePass = commandBuffer.BeginComputePass(
            new StorageBufferReadWriteBinding(_spriteVertexBuffer, true));

            computePass.BindComputePipeline(_computePipeline);
            computePass.BindStorageBuffers(_spriteComputeBuffer);
            computePass.Dispatch(_maxSpriteCount / 64, 1, 1);

            commandBuffer.EndComputePass(computePass);

            var renderPass = commandBuffer.BeginRenderPass(
              new ColorTargetInfo(_renderTarget, _clearColor));

            _batchMatrix *= _worldSpace;

            commandBuffer.PushVertexUniformData(_batchMatrix);

            renderPass.BindGraphicsPipeline(_renderPipeline);
            renderPass.BindVertexBuffers(_spriteVertexBuffer);
            renderPass.BindIndexBuffer(_spriteIndexBuffer, IndexElementSize.ThirtyTwo);

            uint spriteDrawOffset = 0;
            foreach (var spriteData in _spriteData)
            {
                renderPass.BindFragmentSamplers(new TextureSamplerBinding(spriteData.Key, _sampler));
                renderPass.DrawIndexedPrimitives((uint)spriteData.Value.Count * 6, 1, spriteDrawOffset, 0, 0);
                spriteDrawOffset += (uint)spriteData.Value.Count * 6;
            }

            commandBuffer.EndRenderPass(renderPass);
            commandBuffer.Blit(_renderTarget, swapchainTexture, Filter.Nearest);
        }

        _graphicsDevice.Submit(commandBuffer);
        foreach (var list in _spriteData.Values)
        {
            list.Clear();
        }
        _batcher = false;
    }
}