using System.Diagnostics.CodeAnalysis;

namespace AmaAocHelpers.Tools;

[Serializable]
public struct SizeF
{
    public static readonly SizeI Empty = new(0, 0);
    private long width; // Do not rename (binary serialization)
    private long height; // Do not rename (binary serialization)

    public SizeF(SizeF size)
    {
        width = size.width;
        height = size.height;
    }

    public SizeF(long width, long height)
    {
        this.width = width;
        this.height = height;
    }

    public static SizeF operator +(SizeF sz1, SizeF sz2) => Add(sz1, sz2);

    public static SizeF operator -(SizeF sz1, SizeF sz2) => Subtract(sz1, sz2);

    public static SizeF operator *(long left, SizeF right) => Multiply(right, left);

    public static SizeF operator *(SizeF left, long right) => Multiply(left, right);

    public static SizeF operator /(SizeF left, long right)
        => new SizeF(left.width / right, left.height / right);

    public static bool operator ==(SizeF sz1, SizeF sz2) => sz1.Width == sz2.Width && sz1.Height == sz2.Height;

    public static bool operator !=(SizeF sz1, SizeF sz2) => !(sz1 == sz2);

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

    public static SizeF Add(SizeF sz1, SizeF sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

    public static SizeF Subtract(SizeF sz1, SizeF sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is SizeF && Equals((SizeF)obj);

    public readonly bool Equals(SizeF other) => this == other;

    public readonly override int GetHashCode() => HashCode.Combine(Width, Height);

    public readonly override string ToString() => $"{{Width={width}, Height={height}}}";

    private static SizeF Multiply(SizeF size, long multiplier) => new(size.width * multiplier, size.height * multiplier);
}


