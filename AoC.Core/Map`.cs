using System.Runtime.CompilerServices;

namespace AdventOfCode.Runtime;


// X*Y character map
// with information on blockers, if any

// traveller will have options to:
// walk on map
// if at the end of map, or blocking char,
// it will call a function to establish what to do
// but we may also introduce default behaviour,
// like "StayInPlace", "TurnLeft", "TurnRight", "GoBack", etc

public class Map<TType>
{
    protected int sizeX;
    protected int sizeY;
    protected List<TType> mapActual = [];

    public Map(int sizeX, int sizeY)
    {
        SizeX = sizeX;
        SizeY = sizeY;
    }
    public Map(int sizeX, int sizeY, TType defaultFill)
    {
        DefaultFill = defaultFill;
        SizeX = sizeX;
        SizeY = sizeY;
    }
    public Map(int sizeX, int sizeY, Func<int, TType> fillFunc)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        for (int i = 0; i < sizeX * sizeY; i++)
        {
            mapActual[i] = fillFunc(i);
        }
    }
    public Map(int sizeX, int sizeY, Func<int, int, TType> fillFunc)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        for (int y = 0; y < sizeY; y++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                mapActual[i + (y * sizeX)] = fillFunc(i, y);
            }
        }
    }
    public Map(int sizeX, int sizeY, TType[,] fillSrc)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        for (int y = 0; y < Math.Min(sizeY, fillSrc.GetLength(1)); y++)
        {
            for (int i = 0; i < Math.Min(sizeX, fillSrc.GetLength(0)); i++)
            {
                mapActual[i + (y * sizeX)] = fillSrc[y, i];
            }
        }
    }

    public Map(string[] Lines, Func<char, TType> Mapper) : this(Lines.Max(l => l.Length), Lines.Length, (x, y) => Mapper(Lines[y][x]))
    {
    }

    public TType DefaultFill { get; set; } = default;
    public int SizeX
    {
        get => sizeX;
        set
        {
            sizeX = value;
            UpdateMap();

        }
    }
    public int SizeY
    {
        get => sizeY;
        set
        {
            sizeY = value;
            UpdateMap();

        }
    }

    private void UpdateMap()
    {
        if (SizeX <= 0 || SizeY <= 0) return;
        //TODO: we will have to copy the map
        // but for now, just fill it with defaults
        mapActual = Enumerable.Repeat(DefaultFill, SizeX * SizeX).ToList();
    }

    public IList<TType> MapActualLinear => mapActual;

    public TType this[int x, int y] => (x >= 0 && y >= 0 && x < sizeX && y < sizeY) ? mapActual[y * SizeX + x] : DefaultFill;
    public TType this[System.Numerics.Vector2 location] => this[(int)location.X, (int)location.Y];


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set(int x, int y, TType v)
    {
        if (x >= 0 && y >= 0 && mapActual.Count > (y * SizeX + x))
            mapActual[y * SizeX + x] = v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void SetUnsafe(int x, int y, TType v)
    {
        mapActual[y * SizeX + x] = v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Count(Func<int, int, TType, bool> predicate)
    {
        var sum = 0;
        for (var y = 0; y < sizeY; y++)
        {
            for (var x = 0; x < sizeX; x++)
            {
                if (predicate(x, y, mapActual[x + (y * sizeX)]))
                {
                    sum++;
                }
            }
        }
        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private TType GetFast(int x, int y) => (x >= 0 && y >= 0 && x < sizeX && y < sizeY) ? mapActual[y * SizeX + x] : DefaultFill;


    /// <summary>
    /// Returns adjenced cells values.
    /// If Adjenced buffer size == 4, will return cardinal cells, starting with left, then up, right, down.
    /// If buffer size == 8, will return all 8 adjenced values, in same direction, starting from left.
    /// If the buffer is null or size is not 4 or 8, will do nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void GetAdjenced(int x, int y, TType[] Adjenced)
    {
        if (Adjenced == null)
            return;
        if (Adjenced.Length == 4)
        {
            Adjenced[0] = GetFast(x - 1, y);
            Adjenced[1] = GetFast(x, y - 1);
            Adjenced[2] = GetFast(x + 1, y);
            Adjenced[3] = GetFast(x, y + 1);
        }
        if (Adjenced.Length == 8)
        {
            Adjenced[0] = GetFast(x - 1, y);
            Adjenced[1] = GetFast(x - 1, y - 1);
            Adjenced[2] = GetFast(x, y - 1);
            Adjenced[3] = GetFast(x + 1, y - 1);

            Adjenced[4] = GetFast(x + 1, y);
            Adjenced[5] = GetFast(x + 1, y + 1);
            Adjenced[6] = GetFast(x, y + 1);
            Adjenced[7] = GetFast(x - 1, y + 1);
        }
    }


    public void Print(Func<int, int, TType, char> mapper = null)
    {
        for (var y = 0; y < sizeY; y++)
        {
            for (var x = 0; x < sizeX; x++)
            {
                if (mapper != null)
                {
                    Console.Write(mapper(x, y, this[x, y]));
                }
                else
                {
                    Console.Write(this[x, y]);
                }
            }
            Console.WriteLine();
        }
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void CopyTo(Map<TType> Destination)
    {
        if (mapActual.Count != Destination.mapActual.Count)
            return;

        for (var i = 0; i < mapActual.Count; i++)
        {
            Destination.mapActual[i] = mapActual[i];
        }
    }
}

