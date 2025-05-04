using MoonTools.ECS;

namespace MoonWorksShumpExample.Messages;

public readonly record struct Collided(Entity A, Entity B);