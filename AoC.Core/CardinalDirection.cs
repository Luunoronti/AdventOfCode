
namespace AdventOfCode.Runtime;

public struct CardinalDirection
{
    private int _dirflag = 0;
    public static CardinalDirection North = new() { _dirflag = 0 };
    public static CardinalDirection East = new() { _dirflag = 1 };
    public static CardinalDirection South = new() { _dirflag = 2 };
    public static CardinalDirection West = new() { _dirflag = 3 };


    public CardinalDirection()
    {
    }
    private CardinalDirection(int dirFlag) => _dirflag = dirFlag;

    /// <summary>
    /// Returns a new direction that is a result of turning left in relation to this direction.
    /// </summary>
    public readonly CardinalDirection Left => new(_dirflag == 0 ? 3 : _dirflag - 1);
    /// <summary>
    /// Returns a new direction that is a result of turning right in relation to this direction.
    /// </summary>
    public readonly CardinalDirection Right => new(_dirflag == 3 ? 0 : _dirflag + 1);
    /// <summary>
    /// Returns a new direction that is a result of turning back in relation to this direction.
    /// </summary>
    public readonly CardinalDirection Back => new(_dirflag < 2 ? _dirflag + 2 : _dirflag - 2);

    public readonly bool IsNorth => this == North;
    public readonly bool IsEast => this == East;
    public readonly bool IsSouth => this == South;
    public readonly bool IsWest => this == West;

    public readonly Direction Direction => IsSouth ? new(0, 1) : IsNorth ? new(0, -1) : IsEast ? new(1, 0) : IsWest ? new(-1, 0) : new(0, 0);
    public static bool operator ==(CardinalDirection a, CardinalDirection b) => a._dirflag == b._dirflag;
    public static bool operator !=(CardinalDirection a, CardinalDirection b) => a._dirflag != b._dirflag;
    public override readonly bool Equals(object obj) => obj is CardinalDirection direction && _dirflag == direction._dirflag;
    public override readonly int GetHashCode() => HashCode.Combine(_dirflag);
    public override readonly string ToString()
    {
        if (this == North) return nameof(North);
        if (this == South) return nameof(South);
        if (this == West) return nameof(West);
        if (this == East) return nameof(East);
        return base.ToString();
    }
}
