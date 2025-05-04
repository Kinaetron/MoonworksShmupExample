using MoonTools.ECS;
using MoonWorksShumpExample.Components;
using MoonWorksShumpExample.Messages;

namespace MoonWorksShumpExample.Systems;

public class Health : MoonTools.ECS.System
{
    private readonly Filter _damageFilter;

    public Health(World world)
        :base(world)
    {
        _damageFilter =
            FilterBuilder
            .Include<CanTakeDamage>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _damageFilter.Entities)
        {
            var hp = Get<CanTakeDamage>(entity).Value;

            foreach (var message in ReadMessages<Damage>())
            {
                if (message.Entity == entity)
                    hp -= 1;

                if (hp <= 0)
                    Set(entity, new MarkedToDestroy());

                Set(entity, new CanTakeDamage(hp));
            }
        }
    }
}
