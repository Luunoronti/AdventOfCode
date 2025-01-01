using System.Diagnostics.CodeAnalysis;

namespace AmaAocHelpers.Tools;


[Serializable]
public struct SizeI
{
    public static readonly SizeI Empty = new(0, 0);
    private int width; // Do not rename (binary serialization)
    private int height; // Do not rename (binary serialization)

    public SizeI(SizeI size)
    {
        width = size.width;
        height = size.height;
    }

    public SizeI(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public static SizeI operator +(SizeI sz1, SizeI sz2) => Add(sz1, sz2);

    public static SizeI operator -(SizeI sz1, SizeI sz2) => Subtract(sz1, sz2);

    public static SizeI operator *(int left, SizeI right) => Multiply(right, left);

    public static SizeI operator *(SizeI left, int right) => Multiply(left, right);

    public static SizeI operator /(SizeI left, int right)
        => new SizeI(left.width / right, left.height / right);

    public static bool operator ==(SizeI sz1, SizeI sz2) => sz1.Width == sz2.Width && sz1.Height == sz2.Height;

    public static bool operator !=(SizeI sz1, SizeI sz2) => !(sz1 == sz2);

    public readonly bool IsEmpty => width == 0 && height == 0;

    public int Width
    {
        readonly get => width;
        set => width = value;
    }
    public int Height
    {
        readonly get => height;
        set => height = value;
    }

    public static SizeI Add(SizeI sz1, SizeI sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

    public static SizeI Subtract(SizeI sz1, SizeI sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is SizeI && Equals((SizeI)obj);

    public readonly bool Equals(SizeI other) => this == other;

    public readonly override int GetHashCode() => HashCode.Combine(Width, Height);

    public readonly override string ToString() => $"{{Width={width}, Height={height}}}";

    private static SizeI Multiply(SizeI size, int multiplier) => new(size.width * multiplier, size.height * multiplier);
}




