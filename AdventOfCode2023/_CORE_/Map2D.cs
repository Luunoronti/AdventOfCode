using StringSpan = System.ReadOnlySpan<char>;


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

    public T At(int x, int y, out bool outOfRange)
    {
        var pos = y * Width + x;
        if (pos < 0 || pos >= Length)
        {
            outOfRange = true;
            return default;
        }
        outOfRange = false;
        return _map[pos];
    }
    public T At(int x, int y)
    {
        var pos = y * Width + x;
        if (pos < 0 || pos >= Length) return default;
        return _map[pos];
    }

    public T At(int x, int y, T value, out bool outOfRange)
    {
        var pos = y * Width + x;
        if (pos < 0 || pos >= Length)
        {
            outOfRange = true;
            return default;
        }
        outOfRange = false;
        return _map[pos] = value;
    }
    public T At(int x, int y, T value)
    {
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
}
