using System.Diagnostics.CodeAnalysis;

namespace AmaAocHelpers.Tools;

//public struct PointL
//{
//    public long X;
//    public long Y;
//    public PointL(long x, long y)
//    {
//        X = x;
//        Y = y;
//    }
//    public override string ToString() => $"{X}, {Y}";
//    public static bool operator ==(PointL a, PointL b) => Equals(a, b);
//    public static bool operator !=(PointL a, PointL b) => !Equals(a, b);
//    public override bool Equals([NotNullWhen(true)] object? obj) => obj is PointL p && X == p.X && Y == p.Y;
//    public bool Equals(PointL p) => X == p.X && Y == p.Y;
//    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
//}
