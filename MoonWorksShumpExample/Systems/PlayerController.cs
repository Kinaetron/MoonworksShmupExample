using MoonTools.ECS;
using MoonWorksShumpExample.Components;
using System.Numerics;
using Timer = MoonWorksShumpExample.Components.Timer;

namespace MoonWorksShumpExample.Systems;

public class PlayerController : MoonTools.ECS.System
{
    private readonly Filter _playerFilter;
    private readonly BulletController _bulletController;

    public PlayerController(
        World world, 
        BulletController bulletController) :
        base(world)
    {
        _bulletController = bulletController;

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

            var velocity = Get<Velocity>(entity).Value;
            var maxSpeed = Get<MaxSpeed>(entity).Value;
            var accerlation = Get<Accerlation>(entity).Value;

            velocity += direction * accerlation;

            if (velocity.Length() > maxSpeed)
            {
                velocity = Vector2.Normalize(velocity) * maxSpeed;
            }

            if (direction.LengthSquared() <= 0)
            {
                velocity = Vector2.Zero;
            }

            Set(entity, new Velocity(velocity));

            if (inputState.Shoot.IsDown)
            {
                if (HasInRelation<DisableShoot>(entity))
                    return;

                var position = Get<Position>(entity);
                position += new Vector2(3, -10);

                var timer = CreateEntity();
                Set(timer, new Timer(0.1f));
                Relate(timer, entity, new DisableShoot());

                _bulletController.SpawnBullet(position);
            }
        }
    }
}
