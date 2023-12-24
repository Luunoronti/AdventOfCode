using System.Numerics;
using System.Runtime.CompilerServices;

struct Vector3
{
    public static readonly Vector3 zero = new Vector3(0f, 0f, 0f);

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator /(Vector3 a, double d) => new Vector3(a.x / d, a.y / d, a.z / d);


    public override readonly string ToString() => $"{x}, {y}, {z}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Cross(Vector3 lhs, Vector3 rhs) => new(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Dot(Vector3 lhs, Vector3 rhs) => lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
    public double sqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => x * x + y * y + z * z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(Vector3 a, double d) => new Vector3(a.x * d, a.y * d, a.z * d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(double d, Vector3 a) => new Vector3(a.x * d, a.y * d, a.z * d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Magnitude(Vector3 vector)
    {
        return Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Normalize()
    {
        double num = Magnitude(this);
        if (num > 1E-05f)
        {
            this /= num;
        }
        else
        {
            this = zero;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Normalize(Vector3 value)
    {
        double num = Magnitude(value);
        if (num > 1E-05f)
        {
            return value / num;
        }

        return zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator -(Vector3 a) => new Vector3(0f - a.x, 0f - a.y, 0f - a.z);


}
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

struct Vector2
{
    public double x;
    public double y;

    public Vector2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2 operator *(Vector2 a, double b) => new(a.x * b, a.y * b);
    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);
    public override readonly string ToString() => $"{x}, {y}";

    public static double Cross(Vector2 v1, Vector2 v2) => v1.x * v1.y - v1.y * v2.x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Dot(Vector2 lhs, Vector2 rhs) => lhs.x * rhs.x + lhs.y * rhs.y;
    public double sqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => x * x + y * y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Angle(Vector2 from, Vector2 to)
    {
        double num = (double)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
        if (num < 1E-15f)
        {
            return 0f;
        }

        double num2 = Math.Clamp(Dot(from, to) / num, -1f, 1f);
        return (double)Math.Acos(num2) * 57.29578f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsParallel(Vector2 v1, Vector2 v2)
    {
        //2 vectors are parallel if the angle between the vectors are 0 or 180 degrees
        return Angle(v1, v2) == 0f || Angle(v1, v2) == 180f;
    }
    //Are 2 vectors orthogonal?
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOrthogonal(Vector2 v1, Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        return Math.Abs(Dot(v1, v2)) < 0.000001f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsParallel(Vector2 v2)
    {
        //2 vectors are parallel if the angle between the vectors are 0 or 180 degrees
        return Angle(this, v2) == 0f || Angle(this, v2) == 180f;
    }
    //Are 2 vectors orthogonal?
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOrthogonal(Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        return Math.Abs(Dot(this, v2)) < 0.000001f;
    }


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


