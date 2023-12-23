struct Vector3
{
    public double x;
    public double y;
    public double z;

    public Vector3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
    public override readonly string ToString() => $"{x}, {y}, {z}";
}

struct Vector2
{
    public double x;
    public double y;

    public Vector2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);
    public override readonly string ToString() => $"{x}, {y}";
}
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


