using MoonWorks;

namespace MoonWorksShumpExample;

internal class Program
{
    static void Main()
    {
        var windowCreateInfo = new WindowCreateInfo(
             "Shump Game",
             1024,
             768,
             ScreenMode.Windowed
         );

        var framePacingSettings = FramePacingSettings.CreateLatencyOptimized(60);

        var game = new ShumpExample(
            new AppInfo("Shump Game", "ShumpGame"),
            windowCreateInfo,
            framePacingSettings);

        game.Run();
    }
}