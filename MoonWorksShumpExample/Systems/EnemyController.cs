using MoonTools.ECS;
using MoonWorks.Graphics;
using MoonWorksShumpExample.Components;
using MoonWorksShumpExample.Utility;
using System.Numerics;
using Filter = MoonTools.ECS.Filter;

namespace MoonWorksShumpExample.Systems;

public class EnemyController : MoonTools.ECS.System
{
    private readonly System.Random _random;
    private readonly TextureId _textureId;
    private readonly Filter _enemyFilter;

    public EnemyController(World world, TextureId textureId):
        base(world)
    {
        _enemyFilter = FilterBuilder
            .Include<Enemy>()
            .Include<MaxSpeed>()
            .Include<Direction>()
            .Build();

        _random = new System.Random();
        _textureId = textureId;
    }

    public void EnemySpawner()
    {
        var enemy = CreateEntity();
        Set(enemy, new Enemy());
        Set(enemy, _textureId);
        Set(enemy, new Position(new Vector2(_random.Next(0, Dimensions.GameWidth-10), -8)));
        Set(enemy, new Rotation(0));
        Set(enemy, new Size(new Vector2(16, 16)));
        Set(enemy, new Rectangle(0, 0, 16, 16));
        Set(enemy, new MovingSolid());
        Set(enemy, Color.White);
        Set(enemy, new CanTakeDamage(2));
        Set(enemy, new MaxSpeed(_random.Next(1, 2) * Time.TargetFrameRate));
        Set(enemy, new Direction(Vector2.UnitY));
        Set(enemy, new DestroyWhenOutOfBounds());
    }

    public override void Update(TimeSpan delta)
    {
        if(_enemyFilter.Count < 5)
        {
            var spawnCount = 5 - _enemyFilter.Count;

            for (int i = 0; i < spawnCount; i++)
            {
                EnemySpawner();
            }
        }

        foreach (var entity in _enemyFilter.Entities)
        {
            var maxSpeed = Get<MaxSpeed>(entity).Value;
            var direction = Get<Direction>(entity).Value;

            var velocity = direction * maxSpeed;
            Set(entity, new Velocity(velocity));
        }
    }
}
