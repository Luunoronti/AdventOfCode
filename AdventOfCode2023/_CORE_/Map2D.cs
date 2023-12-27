using AdventOfCode2023;
using StringSpan = System.ReadOnlySpan<char>;


enum Adjenced
{
    Center, Left, Right, Top, Bottom, TopLeft, BottomLeft, TopRight, BottomRight
}

class Map2D
{
    public int Width
    {
        get;
    }
    public int Height
    {
        get;
    }
    public int Length
    {
        get;
    }
    public Map2D(int width, int height)
    {
        Width = width;
        Height = height;
        Length = width * height;
    }
}
class Map2D<T> : Map2D
{
    protected T[] _map;

    public Map2D(int width, int height) : base(width, height) => _map = new T[Length];

    public T AtAdjenced(int x, int y, Adjenced adjenced, out bool outOfRange)
    {
        switch (adjenced)
        {
            case Adjenced.Left: return At(x - 1, y, out outOfRange);
            case Adjenced.Right: return At(x + 1, y, out outOfRange);
            case Adjenced.Top: return At(x, y - 1, out outOfRange);
            case Adjenced.Bottom: return At(x, y + 1, out outOfRange);
            case Adjenced.TopLeft: return At(x - 1, y - 1, out outOfRange);
            case Adjenced.BottomLeft: return At(x - 1, y + 1, out outOfRange);
            case Adjenced.TopRight: return At(x + 1, y - 1, out outOfRange);
            case Adjenced.BottomRight: return At(x + 1, y + 1, out outOfRange);
        }
        return At(x, y, out outOfRange);
    }
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
            var v = AtAdjenced(x, y, adjenced, out var outOfRange);
            if (!outOfRange && mapToInt(v, adjenced)) return 1;
            return 0;
        }
    }

    public T At(int x, int y, out bool outOfRange)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            outOfRange = true;
            return default;
        }
        var pos = y * Width + x;
        outOfRange = false;
        return _map[pos];
    }
    public T At(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return default;
        }
        var pos = y * Width + x;
        if (pos < 0 || pos >= Length) return default;
        return _map[pos];
    }

    public T At(int x, int y, T value, out bool outOfRange)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            outOfRange = true;
            return default;
        }
        var pos = y * Width + x;
        outOfRange = false;
        return _map[pos] = value;
    }
    public T At(int x, int y, T value)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return default;
        }
        var pos = y * Width + x;
        if (pos < 0 || pos >= Length) return default;
        return _map[pos] = value;
    }

    public T AtOffset(int offset)
    {
        if (offset < 0 || offset >= Length) return default;
        return _map[offset];
    }

    public T this[int x, int y]
    {
        get => At(x, y);
        set => At(x, y, value);
    }
    public long Count(Func<T, bool> mapper)
    {
        var c = 0L;
        for (var i = 0; i < _map.Length; i++)
            if (mapper(_map[i]))
                c++;
        return c;
    }
    public long Sum(Func<T, long> mapper)
    {
        var c = 0L;
        for (var i = 0; i < _map.Length; i++)
            c += mapper(_map[i]);
        return c;
    }
}
class Map2DSpan<T> : Map2D<T>
{
    public Map2DSpan(int width, int height) : base(width, height) { }
    public Map2DSpan(int width, int height, StringSpan input, Func<char, T> mapFunc) : base(width, height) => Map(input, mapFunc);

    public Span<T> AsSpan() => _map.AsSpan();
    public void Map(StringSpan input, Func<char, T> mapFunc)
    {
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
        if (mapFunc == null) throw new ArgumentNullException(nameof(mapFunc));
        var len = Math.Min(_map.Length, input.Length);
        var span = _map.AsSpan();
        for (int i = 0; i < len; i++)
        {
            span[i] = mapFunc(input[i]);
        }
    }

    public void Map<T2>(Map2DSpan<T2> input, Func<T2, int, int, T> mapFunc)
    {
        if (mapFunc == null) throw new ArgumentNullException(nameof(mapFunc));
        var span = _map.AsSpan();

        for (var y = 0; y < input.Height; y++)
        {
            for (var x = 0; x < input.Width; x++)
            {
                span.At(x, y, Width, mapFunc(input.At(x, y), x, y));
            }
        }
    }
}
