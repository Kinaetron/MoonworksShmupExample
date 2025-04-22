using MoonWorks;
using MoonWorks.Graphics;
using System.Numerics;

namespace MoonWorksShumpExample;

public class ShumpExample : Game
{

    private readonly Texture _boxSprite;
    private readonly Texture _playerSprite1;
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

        _spriteBatch = new SpriteBatch(320, 240, GraphicsDevice, RootTitleStorage, MainWindow);


        _boxSprite = _spriteBatch.CreateTexture("Content/Sprites/Square.png");
        _playerSprite = _spriteBatch.CreateTexture("Content/Sprites/Triangle.png");
        _playerSprite1 = _spriteBatch.CreateTexture("Content/Sprites/Bullet.png");
    }

    protected override void Update(TimeSpan delta) { }

    protected override void Draw(double alpha)
    {
        _spriteBatch.Begin(Color.Black, Matrix4x4.Identity);
        _spriteBatch.Draw(_boxSprite, new Vector2(100, 100), 0, new Vector2(16, 16), Color.White);
        _spriteBatch.Draw(_playerSprite, new Vector2(120, 120), 0, new Vector2(15, 13), Color.White);
        _spriteBatch.Draw(_playerSprite1, new Vector2(120, 150), 0, new Vector2(8, 8), Color.White);
        _spriteBatch.End();
    }
}