using System.Runtime.CompilerServices;

namespace AmaAocHelpers.Tools;

public struct Vector3
{
    public static readonly Vector3 zero = new Vector3(0d, 0d, 0d);

    public double x;
    public double y;
    public double z;

    public Vector3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3(double[] numbers, int offset)
    {
        x = numbers[0 + offset];
        y = numbers[1 + offset];
        z = numbers[2 + offset];
    }
    public Vector3(long[] numbers, int offset)
    {
        x = numbers[0 + offset];
        y = numbers[1 + offset];
        z = numbers[2 + offset];
    }
    public Vector3(int[] numbers, int offset)
    {
        x = numbers[0 + offset];
        y = numbers[1 + offset];
        z = numbers[2 + offset];
    }
    public Vector3(float[] numbers, int offset)
    {
        x = numbers[0 + offset];
        y = numbers[1 + offset];
        z = numbers[2 + offset];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator -(Vector3 a) => new Vector3(0d - a.x, 0d - a.y, 0d - a.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator /(Vector3 a, double d) => new Vector3(a.x / d, a.y / d, a.z / d);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector3(Vector2 v2) => new(v2.x, v2.y, 0);


    public override readonly string ToString() => $"{x}, {y}, {z}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Cross(Vector3 lhs, Vector3 rhs) => new(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Dot(Vector3 lhs, Vector3 rhs) => lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
    public double sqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => x * x + y * y + z * z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(Vector3 a, double d) => new Vector3(a.x * d, a.y * d, a.z * d);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(double d, Vector3 a) => new Vector3(a.x * d, a.y * d, a.z * d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Magnitude(Vector3 vector) => Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Normalize()
    {
        double num = Magnitude(this);
        if (num > 1E-05d)
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
        if (num > 1E-05d)
        {
            return value / num;
        }

        return zero;
    }



}


