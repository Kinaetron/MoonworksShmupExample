using MoonTools.ECS;
using MoonWorksShumpExample.Components;
using MoonWorksShumpExample.Messages;

namespace MoonWorksShumpExample.Systems;

public class Collision : MoonTools.ECS.System
{
    public Collision(World world)
        :base(world)
    {
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var collision in ReadMessages<Collided>())
        {
            if (Has<DestroyOnHit>(collision.A))
            {
                Set(collision.A, new MarkedToDestroy());
            }

            if (Has<DestroyOnHit>(collision.B))
            {
                Set(collision.B, new MarkedToDestroy());
            }
        }
    }
}
