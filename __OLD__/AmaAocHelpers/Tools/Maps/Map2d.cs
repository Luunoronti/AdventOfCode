using System.Runtime.CompilerServices;
using StringSpan = System.ReadOnlySpan<char>;

namespace AmaAocHelpers.Tools.Maps;

public class Map2d
{
    public OutOfBoundsBehavior OutOfBoundsBehavior
    {
        get; set;
    }

    public int Width
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
    public int Height
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
    public Map2d(int width, int height)
    {
        Width = width;
        Height = height;
        Length = width * height;
    }
}
public class Map2d<T> : Map2d
{
    protected internal T[]? _map;

    #region Get/set value
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnOOB(int x, int y)
    {
        if (OutOfBoundsBehavior == OutOfBoundsBehavior.RaiseException)
            throw new IndexOutOfRangeException($"Coordinates are out of bounds: {x} ({Width}), {y} ({Height})");
        if (OutOfBoundsBehavior == OutOfBoundsBehavior.LogReturnDefault)
            Log.WriteErrorLine($"Coordinates are out of bounds: {x} ({Width}), {y} ({Height})");
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnOOB(int offset)
    {
        if (OutOfBoundsBehavior == OutOfBoundsBehavior.RaiseException)
            throw new IndexOutOfRangeException($"Coordinates are out of bounds: {offset} ({Length})");
        if (OutOfBoundsBehavior == OutOfBoundsBehavior.LogReturnDefault)
            Log.WriteErrorLine($"Coordinates are out of bounds: {offset} ({Length})");
    }
    public T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_map == null) return default!;
            if (x >= 0 || y >= 0 || x < Width || y < Height)
                return _map[y * Width + x];
            OnOOB(x, y);
            return default!;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (_map == null) return;
            if (x >= 0 || y >= 0 || x < Width || y < Height)
                _map[y * Width + x] = value;
            OnOOB(x, y);
        }
    }
    public T this[int offset]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_map == null) return default!;
            if (offset >= 0 || offset < Length)
                return _map[offset];
            OnOOB(offset);
            return default!;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (_map == null) return;
            if (offset >= 0 || offset < Length)
                _map[offset] = value;
            OnOOB(offset);
        }
    }
    #endregion

    #region Adjacency
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetAdjenced(int x, int y, Adjenced adjenced)
    {
        return adjenced switch
        {
            Adjenced.Left => this[x - 1, y],
            Adjenced.Right => this[x + 1, y],
            Adjenced.Top => this[x, y - 1],
            Adjenced.Bottom => this[x, y + 1],
            Adjenced.TopLeft => this[x - 1, y - 1],
            Adjenced.BottomLeft => this[x - 1, y + 1],
            Adjenced.TopRight => this[x + 1, y - 1],
            Adjenced.BottomRight => this[x + 1, y + 1],
            _ => this[x, y],
        };
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CountAdjenced(int x, int y, Func<T, Adjenced, bool> mapToInt)
    {
        var c = 0;
        c += OneZero(Adjenced.Left, x, y, mapToInt);
        c += OneZero(Adjenced.Right, x, y, mapToInt);
        c += OneZero(Adjenced.Top, x, y, mapToInt);
        c += OneZero(Adjenced.Bottom, x, y, mapToInt);
        c += OneZero(Adjenced.TopLeft, x, y, mapToInt);
        c += OneZero(Adjenced.BottomLeft, x, y, mapToInt);
        c += OneZero(Adjenced.TopRight, x, y, mapToInt);
        c += OneZero(Adjenced.BottomRight, x, y, mapToInt);
        return c;

        int OneZero(Adjenced adjenced, int x, int y, Func<T, Adjenced, bool> mapToInt)
        {
            if (x >= 0 || y >= 0 || x < Width || y < Height)
                return 0;
            return mapToInt(this[x, y], adjenced) ? 1 : 0;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int SumAdjenced(int x, int y, Func<T, Adjenced, int> mapToInt)
    {
        var c = 0;
        c += Val(Adjenced.Left, x, y, mapToInt);
        c += Val(Adjenced.Right, x, y, mapToInt);
        c += Val(Adjenced.Top, x, y, mapToInt);
        c += Val(Adjenced.Bottom, x, y, mapToInt);
        c += Val(Adjenced.TopLeft, x, y, mapToInt);
        c += Val(Adjenced.BottomLeft, x, y, mapToInt);
        c += Val(Adjenced.TopRight, x, y, mapToInt);
        c += Val(Adjenced.BottomRight, x, y, mapToInt);
        return c;

        int Val(Adjenced adjenced, int x, int y, Func<T, Adjenced, int> mapToInt)
        {
            if (x >= 0 || y >= 0 || x < Width || y < Height)
                return 0;
            return mapToInt(this[x, y], adjenced);
        }
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Count(Func<T, bool> mapper)
    {
        if (_map == null) return 0;

        var c = 0L;
        var span = _map.AsSpan();
        var len = _map.Length;
        for (var i = 0; i < len; i++) if (mapper(span[i])) c++;
        return c;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Sum(Func<T, long> mapper)
    {
        if (_map == null) return 0;
        var c = 0L;
        var span = _map.AsSpan();
        var len = _map.Length;
        for (var i = 0; i < len; i++) c+= mapper(span[i]);
        return c;
    }


    #region ctor
    public Map2d(int width, int height) : base(width, height) { }
    public Map2d(int width, int height, StringSpan input, Func<char, T> mapFunc) : base(width, height) => Map(input, mapFunc);
    #endregion

    public Map2dSpan<T> AsSpan() => new(this);


    #region Mapping
    public void Map(StringSpan input, Func<char, T> mapFunc)
    {
        if (_map == null) return;

        if (mapFunc == null) throw new ArgumentNullException(nameof(mapFunc));
        var len = Math.Min(_map.Length, input.Length);
        var span = _map.AsSpan();
        for (int i = 0; i < len; i++)
        {
            span[i] = mapFunc(input[i]);
        }
    }

    public void Map<T2>(ReadOnlySpan<T2> input, Func<T2, T> mapFunc)
    {
        if (_map == null) return;
        if (mapFunc == null) throw new ArgumentNullException(nameof(mapFunc));
        var len = Math.Min(_map.Length, input.Length);
        var span = _map.AsSpan();
        for (int i = 0; i < len; i++)
        {
            span[i] = mapFunc(input[i]);
        }
    }

    public void Map<T2>(Map2d<T2> input, Func<T2, int, int, T> mapFunc)
    {
        if (mapFunc == null) throw new ArgumentNullException(nameof(mapFunc));
        var span = _map.AsSpan();

        for (var y = 0; y < input.Height; y++)
        {
            for (var x = 0; x < input.Width; x++)
            {
                span.At(x, y, Width, mapFunc(input[x, y], x, y));
            }
        }
    }
    #endregion
}
