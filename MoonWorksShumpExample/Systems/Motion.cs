﻿using MoonTools.ECS;
using MoonWorksShumpExample.Components;
using MoonWorksShumpExample.Utility;
using System.Numerics;

namespace MoonWorksShumpExample.Systems;

public class Motion : MoonTools.ECS.System
{
    private readonly MoonTools.ECS.Filter _solidFilter;
    private readonly MoonTools.ECS.Filter _velocityFilter;

    public Motion(World world)
        :base(world)
    {
        _solidFilter = FilterBuilder
            .Include<Position>()
            .Include<Rectangle>()
            .Include<Solid>()
            .Build();

        _velocityFilter = FilterBuilder
            .Include<Velocity>()
            .Include<Position>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var velocityEntity in _velocityFilter.Entities)
        {
            var position = Get<Position>(velocityEntity);

            if (Has<Rectangle>(velocityEntity) && Has<Solid>(velocityEntity))
            {
                var result = SweepTest(velocityEntity, (float)delta.TotalSeconds);
                Set(velocityEntity, result);
            }
            else
            {
                var velocity = Get<Velocity>(velocityEntity);
                var scaledVelocity = velocity.Value * (float)delta.TotalSeconds;

                Set(velocityEntity, position + scaledVelocity);
            }

            if(Has<DestroyWhenOutOfBounds>(velocityEntity))
            {
                if(position.X < -100 || position.X > Dimensions.GameWidth + 100 ||
                   position.Y < -100 || position.Y > Dimensions.GameHeight + 100)
                {
                    Destroy(velocityEntity);
                }
            }
        }
    }

    private (Entity other, bool hit) CheckSolidCollision(Entity otherEntity, Rectangle rectangle)
    {
        foreach (var entity in _solidFilter.Entities)
        {
            if (entity == otherEntity)
                continue;

            var position = Get<Position>(entity);
            var collider = Get<Rectangle>(entity);
            var worldCollder = collider.GetWorldRect(position);

            if (rectangle.Intersects(worldCollder))
                return (entity, true);
        }

        return (default, false);
    }

    private Position SweepTest(Entity entity, float deltaTime)
    {
        var velocity = Get<Velocity>(entity);
        var position = Get<Position>(entity);
        var rectangle = Get<Rectangle>(entity);

        var movement = new Vector2(velocity.X, velocity.Y) * deltaTime;
        var targetPosition = position + movement;

        var xEnum = new IntegerEnumerator(position.X, targetPosition.X);
        var yEnum = new IntegerEnumerator(position.Y, targetPosition.Y);

        int mostRecentValidXPosition = position.X;
        int mostRecentValidYPosition = position.Y;

        foreach (var x in xEnum)
        {
            var newPosition = new Position(x, position.Y);
            var worldRectangle = rectangle.GetWorldRect(newPosition);

            (var other, var hit) = CheckSolidCollision(entity, worldRectangle);

            if (hit && Has<Solid>(other) && Has<Solid>(entity))
            {
                movement.X = mostRecentValidXPosition - position.X;
                position = position.SetX(position.X);
                break;
            }

            mostRecentValidXPosition = x;
        }

        foreach (var y in yEnum)
        {
            var newPosition = new Position(mostRecentValidXPosition, y);
            var worldRectangle = rectangle.GetWorldRect(newPosition);

            (var other, var hit) = CheckSolidCollision(entity, worldRectangle);

            if (hit && Has<Solid>(other) && Has<Solid>(entity))
            {
                movement.Y = mostRecentValidYPosition - position.Y;
                position = position.SetY(position.Y);
                break;
            }

            mostRecentValidYPosition = y;
        }

        return position + movement;
    }
}
