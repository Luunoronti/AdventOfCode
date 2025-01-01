namespace AmaAocHelpers.Tools;

public struct Rect
{
    public double x;
    public double y;
    public double width;
    public double height;

    public Rect(double x, double y, double width, double height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
    public bool Contains(Vector2 vector2) => vector2.x >= x && vector2.y >= y && vector2.x <= x + width && vector2.y <= y + height;
    public bool Overlaps(Rect other) => (other.width + other.x) > x && other.x < (width + x) && (other.height + other.y) > y && other.y < (height + y);
    public static bool operator !=(Rect lhs, Rect rhs) => !(lhs == rhs);
    public static bool operator ==(Rect lhs, Rect rhs) => lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;

    public override readonly bool Equals(object? obj) => obj != null && obj is Rect rect && this == rect;
    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode();
}

