namespace AdventOfCode.Runtime;

//using Location = DoubleVector2;//System.Numerics.Vector2;


public struct LongVector2
{
    public long X { get; set; }
    public long Y { get; set; }

    public LongVector2(long x, long y)
    {
        X = x;
        Y = y;
    }

    // Length (magnitude) — returns double to preserve precision
    public readonly double Length => Math.Sqrt((X * X) + Y * Y);

    // Length squared
    public readonly long LengthSquared => X * X + Y * Y;

    public readonly long ManhattanLength => Math.Abs(X) + Math.Abs(Y);

    public static long Distance(LongVector2 a, LongVector2 b)
    {
        return (long)(a - b).Length;
    }

    public long Distance(LongVector2 other)
    {
        return (long)(this - other).ManhattanLength;
    }

    // Add
    public static LongVector2 operator +(LongVector2 a, LongVector2 b)
        => new LongVector2(a.X + b.X, a.Y + b.Y);

    // Subtract
    public static LongVector2 operator -(LongVector2 a, LongVector2 b)
        => new LongVector2(a.X - b.X, a.Y - b.Y);

    // Multiply by scalar
    public static LongVector2 operator *(LongVector2 a, long scalar)
        => new LongVector2(a.X * scalar, a.Y * scalar);

    public static LongVector2 operator *(long scalar, LongVector2 a)
        => a * scalar;

    // Dot product
    public static long Dot(LongVector2 a, LongVector2 b)
        => a.X * b.X + a.Y * b.Y;

    // Equality
    public override bool Equals(object obj)
        => obj is LongVector2 other && X == other.X && Y == other.Y;

    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public static bool operator ==(LongVector2 a, LongVector2 b)
        => a.Equals(b);

    public static bool operator !=(LongVector2 a, LongVector2 b)
        => !a.Equals(b);

    public override string ToString()
        => $"({X}, {Y})";
}
