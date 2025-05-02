using System.Numerics;

namespace MoonWorksShumpExample.Components;

public readonly record struct Position
{
    private readonly Vector2 _rawPosition;
    public readonly int X { get; }
    public readonly int Y { get; }

    public Position(float x, float y)
    {
        _rawPosition = new Vector2(x, y);
        X = (int)float.Round(x);
        Y = (int)float.Round(y);
    }

    public Position(int x, int y)
    {
        _rawPosition = new Vector2(x, y);
        X = x;
        Y = y;
    }

    public Position(Vector2 v)
    {
        _rawPosition = v;
        X = (int)float.Round(v.X);
        Y = (int)float.Round(v.Y);
    }

    public Position SetX(int x) =>
        new(x, _rawPosition.Y);

    public Position SetY(int y) =>
        new(_rawPosition.X, y);

    public static Position operator +(Position a, Position b) =>
        new(a._rawPosition + b._rawPosition);

    public static Position operator +(Position a, Vector2 b) =>
        new Position(a._rawPosition + b);

    public override string ToString() =>
        $"({X}, {Y})";

    public readonly Vector2 Value =>
        _rawPosition;
}

