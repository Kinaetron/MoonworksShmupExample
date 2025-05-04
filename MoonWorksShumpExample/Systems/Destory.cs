using MoonTools.ECS;
using MoonWorksShumpExample.Components;

namespace MoonWorksShumpExample.Systems;

public class Destroy : MoonTools.ECS.System
{
    private readonly Filter _destroyFilter;

    public Destroy(World world)
        :base(world)
    {
        _destroyFilter =
           FilterBuilder
           .Include<MarkedToDestroy>()
           .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _destroyFilter.Entities)
        {
            Destroy(entity);
        }
    }
}
