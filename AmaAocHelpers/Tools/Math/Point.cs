using System.Diagnostics.CodeAnalysis;

namespace AmaAocHelpers.Tools;

//public struct Point
//{
//    public int X;
//    public int Y;
//    public Point(int x, int y)
//    {
//        X = x;
//        Y = y;
//    }
//    public override string ToString() => $"{X}, {Y}";
//    public static bool operator ==(Point a, Point b) => Equals(a, b);
//    public static bool operator !=(Point a, Point b) => !Equals(a, b);
//    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Point p && X == p.X && Y == p.Y;
//    public bool Equals(Point p) => X == p.X && Y == p.Y;
//    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
//}
