
using System.Numerics;
using System.Runtime.InteropServices;
using MoonWorks.Storage;
using MoonWorks.Graphics;
using MoonWorks;
using System.Diagnostics;

namespace MoonWorksShumpExample;

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

public class SpriteBatch
{
    private readonly Window _window;
    private readonly GraphicsDevice _graphicsDevice;

    private readonly Sampler _sampler;
    private readonly ComputePipeline _computePipeline;
    private readonly GraphicsPipeline _renderPipeline;

    public SpriteBatch(
        GraphicsDevice graphicsDevice, 
        TitleStorage titleStorage,
        Window window)
    {
        _window = window;
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
                            BlendState = ColorTargetBlendState.Opaque
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

    }
}