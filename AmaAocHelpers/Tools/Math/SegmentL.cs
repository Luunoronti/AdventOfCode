namespace AmaAocHelpers.Tools;

public struct SegmentL
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

