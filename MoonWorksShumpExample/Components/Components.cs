using System.Numerics;

namespace MoonWorksShumpExample.Components;

public readonly record struct Player();
public readonly record struct TextureId(int Value);
public readonly record struct Position(Vector2 Value);
public readonly record struct Rotation(float Value);
public readonly record struct Size(Vector2 Value);