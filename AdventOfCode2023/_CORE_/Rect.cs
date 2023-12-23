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
    public bool Overlaps(Rect other) => (other.width + other.x) > x && other.x < (width + x) && (other.height + other.y) > y && other.y < (height + y);
    public static bool operator !=(Rect lhs, Rect rhs) => !(lhs == rhs);
    public static bool operator ==(Rect lhs, Rect rhs) => lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;

}


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
    public bool Contains(PointL point) => point.X >= x && point.Y >= y && point.X <= x + width && point.Y <= y + height;
    public bool Contains(Vector2Long vector2) => vector2.x >= x && vector2.y >= y && vector2.x <= x + width && vector2.y <= y + height;



    public bool Overlaps(RectL other) => (other.width + other.x) > x && other.x < (width + x) && (other.height + other.y) > y && other.y < (height + y);
    public static bool operator !=(RectL lhs, RectL rhs) => !(lhs == rhs);
    public static bool operator ==(RectL lhs, RectL rhs) => lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;

}

public struct Segment
{
    public Vector2Long Start;
    public Vector2Long End;
    public bool IsVertical => Start.x == End.x;
    public bool IsHorizontal => Start.y == End.y;

    public override string ToString()
    {
        return $"[{Start}] [{End}] ([{End - Start}] ({(IsVertical ? "vertical" : "horizontal")}))";
    }
}

