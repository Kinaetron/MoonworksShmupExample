using MoonWorks;

namespace MoonWorksShumpExample;

internal class Program
{
    static void Main()
    {
        var windowCreateInfo = new WindowCreateInfo(
             "Example Game",
             1280,
             720,
             ScreenMode.Windowed
         );

        var framePacingSettings = FramePacingSettings.CreateLatencyOptimized(60);

        var game = new ShumpExample(
            new AppInfo("Example Game", "ExampleGame"),
            windowCreateInfo,
            framePacingSettings);

        game.Run();
    }
}