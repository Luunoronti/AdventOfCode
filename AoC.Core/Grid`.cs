namespace AoC.Core;

public class Grid<T>
{
    private readonly T[,] _data;

    public int Width { get; }
    public int Height { get; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _data = new T[width, height];
    }

    public T this[int x, int y]
    {
        get => _data[x, y];
        set => _data[x, y] = value;
    }
}