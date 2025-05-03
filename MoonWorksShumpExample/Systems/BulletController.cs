using MoonTools.ECS;
using MoonWorks.Graphics;
using MoonWorksShumpExample.Components;
using MoonWorksShumpExample.Utility;
using System.Numerics;

namespace MoonWorksShumpExample.Systems;

public class BulletController : MoonTools.ECS.System
{
    private readonly TextureId _textureId;
    private readonly MoonTools.ECS.Filter _bulletFilter;

    public BulletController(
        World world, 
        TextureId textureId):
        base(world)
    {
        _textureId = textureId;

        _bulletFilter =
            FilterBuilder
            .Include<Bullet>()
            .Include<MaxSpeed>()
            .Include<Direction>()
            .Build();
    }

    public void SpawnBullet(
        Position position,
        Direction direction)
    {
        var bullet = CreateEntity();
        Set(bullet, new Bullet());
        Set(bullet, _textureId);
        Set(bullet, new Rotation(0));
        Set(bullet, new Size(new Vector2(8, 8)));
        Set(bullet, Color.White);
        Set(bullet, new MaxSpeed(4.0f * Time.TargetFrameRate));
        Set(bullet, new Rectangle(0, 0, 8, 8));
        Set(bullet, position);
        Set(bullet, direction);
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _bulletFilter.Entities)
        {
            var maxSpeed = Get<MaxSpeed>(entity).Value;
            var direction = Get<Direction>(entity).Value;

            var velocity = direction * maxSpeed;
            Set(entity, new Velocity(velocity));
        }
    }
}
