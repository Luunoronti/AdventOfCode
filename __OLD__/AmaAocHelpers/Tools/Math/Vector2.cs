using System.Runtime.CompilerServices;
using System;
namespace AmaAocHelpers.Tools;

public struct Vector2
{
    public static readonly Vector2 zero = new Vector2(0d, 0d);

    public double x;
    public double y;

    public Vector2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
    public Vector2(Vector3 v3)
    {
        this.x = v3.x;
        this.y = v3.y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 operator *(Vector2 a, double b) => new(a.x * b, a.y * b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 operator /(Vector2 a, double d) => new Vector2(a.x / d, a.y / d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector2(Vector3 v3) => new(v3.x, v3.y);


    public override readonly string ToString() => $"{x}, {y}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Cross(Vector2 v1, Vector2 v2) => v1.x * v1.y - v1.y * v2.x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Dot(Vector2 lhs, Vector2 rhs) => lhs.x * rhs.x + lhs.y * rhs.y;
    public double sqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => x * x + y * y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Angle(Vector2 from, Vector2 to)
    {
        double num = (double)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
        if (num < 1E-15d)
        {
            return 0d;
        }

        double num2 = Math.Clamp(Dot(from, to) / num, -1d, 1d);
        return (double)Math.Acos(num2) * 57.29578; // 57.295... == 1.000 rad (radian)
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsParallel(Vector2 v1, Vector2 v2)
    {
        //2 vectors are parallel if the angle between the vectors are 0 or 180 degrees
        var a = Angle(v1, v2);
        return a == 0d || a == 180;
    }
    //Are 2 vectors orthogonal?
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOrthogonal(Vector2 v1, Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        return Math.Abs(Dot(v1, v2)) < 0.000001;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsParallel(Vector2 v2)
    {
        //2 vectors are parallel id the angle between the vectors are 0 or 180 degrees
        var a = Angle(this, v2);
        return a == 0d || a == 180;
    }
    //Are 2 vectors orthogonal?
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOrthogonal(Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        return Math.Abs(Dot(this, v2)) < 0.000001;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Magnitude(Vector2 vector) => Math.Sqrt(vector.x * vector.x + vector.y * vector.y);

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
    public static Vector2 Normalize(Vector2 value)
    {
        double num = Magnitude(value);
        if (num > 1E-05d)
        {
            return value / num;
        }

        return zero;
    }


}
