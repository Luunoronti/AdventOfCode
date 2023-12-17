class Map1D
{
    public int Width { get; }
    public int Height { get; }
    public int Length { get; }
    public Map1D(int width, int height)
    {
        Width = width;
        Height = height;
        Length = width * height;
    }
}
class Map1D<T> : Map1D
{
    private T[] _map;

    public Map1D(int width, int height) : base(width, height) => _map = new T[Length];

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

}
class Map1DSpan<T> : Map1D
{
    public Map1DSpan(int width, int height) : base(width, height) { }

    public Span<T> ToSpan() => default;
}



