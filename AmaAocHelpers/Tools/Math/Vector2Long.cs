namespace AmaAocHelpers.Tools;

public struct Vector2Long
{
    public long x;
    public long y;

    public Vector2Long(long x, long y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2Long operator +(Vector2Long a, Vector2Long b) => new(a.x + b.x, a.y + b.y);
    public static Vector2Long operator -(Vector2Long a, Vector2Long b) => new(a.x - b.x, a.y - b.y);
    public override readonly string ToString() => $"{x}, {y}";
}
