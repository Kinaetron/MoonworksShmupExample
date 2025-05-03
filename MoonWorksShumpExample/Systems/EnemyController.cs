using MoonTools.ECS;
using MoonWorksShumpExample.Components;
using System.Numerics;

namespace MoonWorksShumpExample.Systems;

public class EnemyController : MoonTools.ECS.System
{
    private readonly Filter _enemyFilter;

    public EnemyController(World world) :
        base(world)
    {
        _enemyFilter = FilterBuilder
            .Include<Enemy>()
            .Include<Velocity>()
            .Include<Accerlation>()
            .Include<MaxSpeed>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _enemyFilter.Entities)
        {
            var direction = Vector2.Zero;
            direction.X += 10;
            direction.Y += 10;
    

            var deltaTime = (float)delta.TotalSeconds;
            var velocity = Get<Velocity>(entity).Value;
            var maxSpeed = Get<MaxSpeed>(entity).Value;
            var accerlation = Get<Accerlation>(entity).Value;

            velocity += direction * accerlation * deltaTime;

            if (velocity.Length() > maxSpeed * deltaTime)
            {
                velocity = Vector2.Normalize(velocity) * maxSpeed * deltaTime;
            }

            if (direction.LengthSquared() <= 0)
            {
                velocity = Vector2.Zero;
            }

            Set(entity, new Velocity(velocity));
        }
    }
}
