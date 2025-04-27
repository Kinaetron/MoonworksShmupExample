using MoonTools.ECS;
using MoonWorksShumpExample.Components;

namespace MoonWorksShumpExample.Systems;

public class Motion : MoonTools.ECS.System
{
    private readonly Filter _velocityFilter;

    public Motion(World world)
        :base(world)
    {
        _velocityFilter = FilterBuilder
            .Include<Velocity>()
            .Include<Position>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var velocityEntity in _velocityFilter.Entities)
        {
            var velocity = Get<Velocity>(velocityEntity).Value;
            var position = Get<Position>(velocityEntity).Value;

            position += velocity;

            Set(velocityEntity, new Position(position));
        }
    }
}
