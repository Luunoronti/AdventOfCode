using System.Diagnostics.CodeAnalysis;

namespace AmaAocHelpers.Tools;

[Serializable]
public struct SizeL
{
    public static readonly SizeI Empty = new(0, 0);
    private long width; // Do not rename (binary serialization)
    private long height; // Do not rename (binary serialization)

    public SizeL(SizeL size)
    {
        width = size.width;
        height = size.height;
    }

    public SizeL(long width, long height)
    {
        this.width = width;
        this.height = height;
    }

    public static SizeL operator +(SizeL sz1, SizeL sz2) => Add(sz1, sz2);

    public static SizeL operator -(SizeL sz1, SizeL sz2) => Subtract(sz1, sz2);

    public static SizeL operator *(long left, SizeL right) => Multiply(right, left);

    public static SizeL operator *(SizeL left, long right) => Multiply(left, right);

    public static SizeL operator /(SizeL left, long right)
        => new SizeL(left.width / right, left.height / right);

    public static bool operator ==(SizeL sz1, SizeL sz2) => sz1.Width == sz2.Width && sz1.Height == sz2.Height;

    public static bool operator !=(SizeL sz1, SizeL sz2) => !(sz1 == sz2);

    public readonly bool IsEmpty => width == 0 && height == 0;

    public long Width
    {
        readonly get => width;
        set => width = value;
    }
    public long Height
    {
        readonly get => height;
        set => height = value;
    }

    public static SizeL Add(SizeL sz1, SizeL sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

    public static SizeL Subtract(SizeL sz1, SizeL sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is SizeL && Equals((SizeL)obj);

    public readonly bool Equals(SizeL other) => this == other;

    public readonly override int GetHashCode() => HashCode.Combine(Width, Height);

    public readonly override string ToString() => $"{{Width={width}, Height={height}}}";

    private static SizeL Multiply(SizeL size, long multiplier) => new(size.width * multiplier, size.height * multiplier);
}
