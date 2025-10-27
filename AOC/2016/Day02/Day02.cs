
using TermGlass;
using TermGlass.DemoWorld;
using Windows.Media.Streaming.Adaptive;

namespace Year2016;



// map

// X*Y character map
// with information on blockers, if any

// traveller will have options to:
// walk on map
// if at the end of map, or blocking char,
// it will call a function to establish what to do
// but we may also introduce default behaviour,
// like "StayInPlace", "TurnLeft", "TurnRight", "GoBack", etc

class Map<TType>
{
    private int sizeX;
    private int sizeY;
    private List<TType> mapActual = [];

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
}

class Day02
{
    private sealed class FillState
    {
        public Queue<(int x, int y)> Q = new();
        public bool[,] Visited = null!;
        public int Steps = 0;
        public int Filled = 0;
    }
    private sealed class ArrayWorld : IWorldSource
    {
        private readonly Cell[,] _cells;
        public int Width
        {
            get;
        }
        public int Height
        {
            get;
        }

        public ArrayWorld(Cell[,] cells)
        {
            _cells = cells;
            Width = cells.GetLength(0);
            Height = cells.GetLength(1);
        }

        public Cell? GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return _cells[x, y];
        }


    }


    public string Part1(PartInput Input)
    {
        // Old();

        int W = 200, H = 200;
        var cells = new Cell[W, H];
        GenerateMaze(cells);
        var fill = InitFill(cells, W, H, 0, 0);
        var world = new ArrayWorld(cells);


        var win = Window.Create(
           x: 5, y: 2, w: 26, h: 6,
           bg: new Rgb(15, 15, 20), bgAlpha: 210,
           z: 50,
           content: (buf, self) => DrawFillStatus(buf, self, fill)
       );
        var cfg = new VizConfig
        {
            ColorMode = ColorMode.TrueColor,
            TargetFps = 60,
            AutoPlay = false,
            AutoStepPerSecond = 60,
            Layers = UiLayers.All
        };

        Visualizer.Run(cfg,
            process: () =>
            {
                for (int i = 0; i < 50; i++)
                    StepFill(cells, fill, W, H);
                return true;
            },
            (frame, completed) =>
            {
                frame.DrawWorld(world);
            },
            info: (x, y) =>
            {
                if (x < 0 || y < 0 || x >= W || y >= H) return null;
                var c = world.GetCell(x, y)!.Value;
                return $"({x},{y}) '{c.Ch}'";
            });


        long response = Input.LineWidth;
        return response.ToString();
    }

    void Old()
    {
        // Example: Move 'X' across the buffer for 10 seconds
        var world = new DemoWorld(300, 150);

        // Proste okno 30x8 w (5,3), lekko przezroczyste
        var win = Window.Create(
            x: 5, y: 3, w: 30, h: 8,
            bg: new Rgb(15, 15, 20), bgAlpha: 210,
            border: new Rgb(255, 255, 255), borderAlpha: 200,
            z: 10,
            content: (buf, self) =>
            {
                // Wypełnij tytuł i kilka linii treści (bez zmiany tła)
                Renderer.PutTextKeepBg(buf, self.X + 2, self.Y, "[ Demo Window ]", new Rgb(255, 230, 120));
                Renderer.PutTextKeepBg(buf, self.X + 2, self.Y + 2, "Hello from Window!", new Rgb(240, 240, 240));
                Renderer.PutTextKeepBg(buf, self.X + 2, self.Y + 3, "Press C to toggle color mode.", new Rgb(200, 220, 255));
            });

        Visualizer.Run(new VizConfig
        {
            ColorMode = ColorMode.TrueColor,
            TargetFps = 60,
            AutoPlay = true,
            Layers = UiLayers.All
        },
        process: () => 
        {
            return true;
        },
        (frame, completed) =>
        {
            frame.DrawWorld(world);

            var (wx, wy) = frame.ScreenToWorld(frame.Input.MouseX, frame.Input.MouseY);
            frame.DrawCircleWorld(wx, wy, 20, '•', Rgb.Yellow, Rgb.Transparent);
        },
        info: (x, y) =>
        {
            // x,y to indeksy komórek świata (zoom<1 → lewy-górny z bloku)
            if (x < 0 || y < 0 || x >= world.Width || y >= world.Height) return null;
            return @$"Cell=({x},{y})  
Char='{world.GetCell(x, y)?.Ch}'";
        });
    }






    private static void SetCell(Cell[,] cells, int x, int y, Cell c)
    {
        if (x < 0 || y < 0 || x >= cells.GetLength(0) || y >= cells.GetLength(1)) return;
        cells[x, y] = c;
    }

    private static Cell GetCell(Cell[,] cells, int x, int y)
    {
        return cells[x, y];
    }

    public string Part2(PartInput Input)
    {
        long response = Input.LineWidth;
        return response.ToString();
    }

    private void DrawFillStatus(CellBuffer buf, Window self, FillState st)
    {
        Renderer.PutTextKeepBg(buf, self.X + 2, self.Y, "[ Fill Status ]", new Rgb(255, 230, 120));
        Renderer.PutTextKeepBg(buf, self.X + 2, self.Y + 2, $"Steps:  {st.Steps}", new Rgb(230, 230, 230));
        Renderer.PutTextKeepBg(buf, self.X + 2, self.Y + 3, $"Filled: {st.Filled}", new Rgb(230, 230, 230));
    }


    private bool StepFill(Cell[,] cells, FillState st, int w, int h)
    {
        if (st.Q.Count == 0) return false;

        var (x, y) = st.Q.Dequeue();
        st.Steps++;

        SetCell(cells, x, y, new Cell('·', new Rgb(255, 255, 120), new Rgb(40, 110, 210)));
        st.Filled++;

        foreach (var (nx, ny) in new (int, int)[] { (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1) })
        {
            if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
            if (st.Visited[nx, ny]) continue;

            var c = GetCell(cells, nx, ny);
            if (c.Ch == ' ' || c.Ch == '·')
            {
                st.Visited[nx, ny] = true;
                st.Q.Enqueue((nx, ny));
            }
        }

        return st.Q.Count > 0;
    }


    private FillState InitFill(Cell[,] cells, int w, int h, int sx = 0, int sy = 0)
    {
        // znajdź najbliższą od (sx,sy) komórkę typu ' ' (korytarz)
        (int x, int y) start = (sx, sy);
        bool found = false;
        for (int y = sy; y < h && !found; y++)
            for (int x = sx; x < w && !found; x++)
            {
                var c = GetCell(cells, x, y);
                if (c.Ch == ' ') { start = (x, y); found = true; }
            }
        if (!found) start = (0, 0);

        var st = new FillState
        {
            Visited = new bool[w, h]
        };
        st.Q.Enqueue(start);
        st.Visited[start.x, start.y] = true;
        return st;
    }


    private void GenerateMaze(Cell[,] cells)
    {
        int w = cells.GetLength(0);
        int h = cells.GetLength(1);

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                SetCell(cells, x, y, new Cell('#', new Rgb(90, 90, 90), new Rgb(20, 20, 20)));

        for (int y = 0; y < h; y += 2)
            for (int x = 0; x < w; x += 2)
                SetCell(cells, x, y, new Cell(' ', new Rgb(220, 220, 220), new Rgb(15, 15, 18)));

        var rnd = new Random(1);
        for (int y = 0; y < h; y += 2)
        {
            for (int x = 0; x < w; x += 2)
            {
                bool canE = (x + 2 < w);
                bool canS = (y + 2 < h);
                if (!canE && !canS) continue;

                if (canE && (!canS || rnd.Next(2) == 0))
                    SetCell(cells, x + 1, y, new Cell(' ', new Rgb(220, 220, 220), new Rgb(15, 15, 18)));
                else if (canS)
                    SetCell(cells, x, y + 1, new Cell(' ', new Rgb(220, 220, 220), new Rgb(15, 15, 18)));
            }
        }
    }

}
