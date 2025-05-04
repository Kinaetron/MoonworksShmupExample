using MoonTools.ECS;
using Timer = MoonWorksShumpExample.Components.Timer;

namespace MoonWorksShumpExample.Systems;

public class Timing : MoonTools.ECS.System
{
    private readonly Filter _timerFilter;

    public Timing(World world):
        base(world)
    {
        _timerFilter =
            FilterBuilder
            .Include<Timer>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _timerFilter.Entities)
        {
            var timer = Get<Timer>(entity);
            var time = timer.Time - (float)delta.TotalSeconds;

            if (time <= 0)
            {
                Destroy(entity);
                return;
            }

            Set(entity, timer with { Time = time });
        }
    }
}
