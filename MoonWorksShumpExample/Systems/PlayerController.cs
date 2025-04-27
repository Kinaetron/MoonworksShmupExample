using MoonTools.ECS;
using MoonWorksShumpExample.Components;
using System.Numerics;

namespace MoonWorksShumpExample.Systems;

public class PlayerController : MoonTools.ECS.System
{
    private readonly Filter _playerFilter;

    public PlayerController(World world) :
        base(world)
    {
        _playerFilter = FilterBuilder
            .Include<Player>()
            .Include<Velocity>()
            .Include<Accerlation>()
            .Include<MaxSpeed>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _playerFilter.Entities)
        {
            var direction = Vector2.Zero;
            var inputState = Get<InputState>(entity);

            if(inputState.Left.IsDown)
            {
                direction.X = -1;
            }
            else if(inputState.Right.IsDown)
            {
                direction.X = 1;
            }

            if(inputState.Up.IsDown)
            {
                direction.Y -= 1;
            }
            else if(inputState.Down.IsDown)
            {
                direction.Y = 1;
            }

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
