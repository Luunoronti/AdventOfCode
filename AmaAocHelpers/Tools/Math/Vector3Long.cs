namespace AmaAocHelpers.Tools;

struct Vector3Long
{
    public long x;
    public long y;
    public long z;

    public Vector3Long(long x, long y, long z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3Long operator +(Vector3Long a, Vector3Long b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Vector3Long operator -(Vector3Long a, Vector3Long b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
    public override readonly string ToString() => $"{x}, {y}, {z}";
}
