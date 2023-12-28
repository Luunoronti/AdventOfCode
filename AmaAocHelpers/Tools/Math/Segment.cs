namespace AmaAocHelpers.Tools;

public struct Segment
{
    public Vector2 Start;
    public Vector2 End;
    public bool IsVertical => Start.x == End.x;
    public bool IsHorizontal => Start.y == End.y;

    public override string ToString()
    {
        return $"[{Start}] [{End}] ([{End - Start}] ({(IsVertical ? "vertical" : "horizontal")}))";
    }
}

