using MoonWorks;
using MoonWorks.Graphics;
using System.Numerics;

namespace MoonWorksShumpExample;

public class ShumpExample : Game
{

    private readonly Texture _playerSprite;
    private readonly SpriteBatch _spriteBatch;

    public ShumpExample(
        AppInfo appInfo,
        WindowCreateInfo windowCreateInfo,
        FramePacingSettings framePacingSettings
        ) : base(
            appInfo,
            windowCreateInfo,
            framePacingSettings,
            ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.MSL | ShaderFormat.DXBC)
    {
        ShaderCross.Initialize();

        _playerSprite =  Texture.Create2D(
            GraphicsDevice,
            "Content/Sprites/Triangle.png",
            15,
            13,
            TextureFormat.R8G8B8A8Unorm,
            TextureUsageFlags.Sampler
            );

        _spriteBatch = new SpriteBatch(GraphicsDevice, RootTitleStorage, MainWindow);
    }

    protected override void Update(TimeSpan delta) { }

    protected override void Draw(double alpha)
    {
        CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
        Texture swapchainTexture = cmdbuf.AcquireSwapchainTexture(MainWindow);
        if (swapchainTexture != null)
        {
            var renderPass = cmdbuf.BeginRenderPass(
                new ColorTargetInfo(swapchainTexture, Color.CornflowerBlue)
            );
            cmdbuf.EndRenderPass(renderPass);
        }
        GraphicsDevice.Submit(cmdbuf);

        _spriteBatch.Draw(_playerSprite, Vector2.Zero, 0, Vector2.Zero, Color.White);
        _spriteBatch.Draw(_playerSprite, Vector2.Zero, 0, Vector2.Zero, Color.White);
    }
}