using System.Numerics;

namespace MoonWorksShumpExample.Components;

public readonly record struct Player();
public readonly record struct TextureId(int Value);
public readonly record struct Position(Vector2 Value);
public readonly record struct Rotation(float Value);
public readonly record struct Size(Vector2 Value);
public readonly record struct Velocity(Vector2 Value);
public readonly record struct Accerlation(float Value);
public readonly record struct MaxSpeed(float Value);
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
            (int)position.Value.X + X,
            (int)position.Value.Y + Y,
            Width,
            Height);
    }

    public RaycasResult IntersectsMoving(Rectangle other, Vector2 velocity)
    {
        var result = new RaycasResult { Hit = false, THitNear = 0 };

        if (velocity == Vector2.Zero)
            return result;

        var invVelocity = new Vector2(
           velocity.X != 0 ? 1.0f / velocity.X : float.MaxValue,
           velocity.Y != 0 ? 1.0f / velocity.Y : float.MaxValue
           );

        var tNear = new Vector2(
           (other.Left - Right) * invVelocity.X,
           (other.Top - Bottom) * invVelocity.Y);

        var tFar = new Vector2(
          (other.Right - Left) * invVelocity.X,
          (other.Bottom - Top) * invVelocity.Y);

        if (tNear.X > tFar.X) (tNear.X, tFar.X) = (tFar.X, tNear.X);
        if (tNear.Y > tFar.Y) (tNear.Y, tFar.Y) = (tFar.Y, tNear.Y);

        if (tNear.X > tFar.X || tNear.Y > tFar.Y)
            return result;

        result.THitNear = float.Max(tNear.X, tNear.Y);
        var tHitFar = float.Min(tFar.X, tFar.Y);

        if (tHitFar < 0 || result.THitNear > 1)
            return result;

        result.Hit = true;
        result.ContactPoint = new Vector2(X, Y) + velocity * result.THitNear;

        if (tNear.X > tNear.Y)
        {
            result.Normal = velocity.X < 0 ? 
                new Vector2(1, 0) : 
                new Vector2(-1, 0);
        }
        else
        {
            result.Normal = velocity.Y < 0 ? 
                new Vector2(0, 1) : 
                new Vector2(0, -1);
        }

        return result;
    }
}

public record struct RaycasResult(
    bool Hit, 
    float THitNear, 
    Vector2 ContactPoint,
    Vector2 Normal);