namespace AdventOfCode.Runtime;

public struct DoubleVector2
{
    public double X { get; set; }
    public double Y { get; set; }

    public DoubleVector2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double Length => Math.Sqrt(X * X + Y * Y);
    public double LengthSquared => X * X + Y * Y;

    public static DoubleVector2 operator +(DoubleVector2 a, DoubleVector2 b)
        => new DoubleVector2(a.X + b.X, a.Y + b.Y);

    public static DoubleVector2 operator -(DoubleVector2 a, DoubleVector2 b)
        => new DoubleVector2(a.X - b.X, a.Y - b.Y);

    public static DoubleVector2 operator *(DoubleVector2 a, double scalar)
        => new DoubleVector2(a.X * scalar, a.Y * scalar);

    public static DoubleVector2 operator *(double scalar, DoubleVector2 a)
        => a * scalar;

    public static double Dot(DoubleVector2 a, DoubleVector2 b)
        => a.X * b.X + a.Y * b.Y;

    public override string ToString()
        => $"({X:F3}, {Y:F3})"; // Rounded for readability
};
