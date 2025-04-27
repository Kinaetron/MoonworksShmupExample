using MoonWorks;
using MoonWorks.Graphics;
using MoonWorksShumpExample.GameStates;

namespace MoonWorksShumpExample;

public class ShumpExample : Game
{
    private readonly GameplayState _gameplayState;

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
        _gameplayState = new GameplayState(this);

        _gameplayState.Start();
    }

    protected override void Update(TimeSpan delta) 
    {
        _gameplayState.Update(delta);
    }

    protected override void Draw(double alpha)
    {
        _gameplayState.Draw(alpha);
    }
}