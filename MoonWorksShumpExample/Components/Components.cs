using System.Numerics;

namespace MoonWorksShumpExample.Components;

public readonly record struct Enemy();
public readonly record struct Player();
public readonly record struct Bullet();
public readonly record struct TextureId(int Value);
public readonly record struct Rotation(float Value);
public readonly record struct Size(Vector2 Value);
public readonly record struct Accerlation(float Value);
public readonly record struct MaxSpeed(float Value);
public readonly record struct Direction(Vector2 Value);
public readonly record struct Solid();
public readonly record struct DisableShoot();
public readonly record struct DestroyWhenOutOfBounds();
public readonly record struct DestroyOnHit();
public readonly record struct SweepCollision();
public readonly record struct MarkedToDestroy();
public readonly record struct CanTakeDamage(int Value);
public readonly record struct CanDealDamage();
public readonly record struct Rectangle(int X, int Y, int Width, int Height)
{
    public int Left => X;
    public int Right => X + Width;
    public int Top => Y;
    public int Bottom => Y + Height;

    public bool Intersects(Rectangle other)
    {
        return
            other.Left < Right &&
            Left < other.Right &&
            other.Top < Bottom &&
            Top < other.Bottom;
    }

    public Rectangle GetWorldRect(Position position)
    {
        return new Rectangle(
            position.X + X,
            position.Y + Y,
            Width,
            Height);
    }
}