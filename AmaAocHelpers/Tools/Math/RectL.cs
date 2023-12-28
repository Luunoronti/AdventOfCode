namespace AmaAocHelpers.Tools;

public struct RectL
{
    public long x;
    public long y;
    public long width;
    public long height;

    public RectL(long x, long y, long width, long height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
    public bool Contains(Vector2Long vector2) => vector2.x >= x && vector2.y >= y && vector2.x <= x + width && vector2.y <= y + height;



    public bool Overlaps(RectL other) => (other.width + other.x) > x && other.x < (width + x) && (other.height + other.y) > y && other.y < (height + y);
    public static bool operator !=(RectL lhs, RectL rhs) => !(lhs == rhs);
    public static bool operator ==(RectL lhs, RectL rhs) => lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
    public override readonly bool Equals(object? obj) => obj != null && obj is RectL rect && this == rect;
    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode();

}