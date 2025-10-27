namespace AdventOfCode.Extensions;

public static class Vector2DExtentions
{
    public static Vector2D ToFlooredLong(this Vector2D vector) => new((long)Math.Floor(vector.X), (long)Math.Floor(vector.Y));
    public static double ManhattanDistance(this Vector2D vector) => Math.Abs(vector.X) + Math.Abs(vector.Y);
    public static double ManhattanDistance(this Vector2D vector, Vector2D other) => Math.Abs(vector.X - other.X) + Math.Abs(vector.Y - other.Y);

}