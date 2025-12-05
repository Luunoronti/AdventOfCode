namespace AoC;

public static class Solver
{
    const byte MAX = 4;

    static int[] dirX = { -1, 0, 1, -1, 1, -1, 0, 1 };
    static int[] dirY = { -1, -1, -1, 0, 0, 1, 1, 1 };


    public static void PrepareMaps(string[] Lines, out int SizeX, out int SizeY, out int Size, out bool[] Rolls, out byte[] Counts)
    {
        SizeY = Lines.Length;
        SizeX = Lines[0].Length;
        Size = SizeY * SizeX;

        Rolls = new bool[Size];
        Counts = new byte[Size];

        for (var y = 0; y < SizeY; y++)
        {
            var line = Lines[y];
            var row = y * SizeX;
            for (var x = 0; x < SizeX; x++)
            {
                if (line[x] == '@')
                    Rolls[row + x] = true;
            }
        }

        for (var y = 0; y < SizeY; y++)
        {
            var row = y * SizeX;
            for (var x = 0; x < SizeX; x++)
            {
                var idx = row + x;
                if (!Rolls[idx]) continue;

                byte c = 0;
                for (var d = 0; d < 8; d++)
                {
                    var nx = x + dirX[d];
                    var ny = y + dirY[d];

                    if ((uint)nx >= (uint)SizeX || (uint)ny >= (uint)SizeY) continue;

                    if (Rolls[ny * SizeX + nx])
                        c++;
                }
                Counts[idx] = c;
            }
        }
    }


    public static object? SolvePart1(string[] Lines)
    {
        PrepareMaps(Lines, out var SizeX, out var SizeY, out var Size, out var Rolls, out var Counts);

        long count = 0;
        for (int k = 0; k < Size; k++)
        {
            if (Rolls[k] && Counts[k] < MAX)
                count++;
        }
        return count;
    }

    public static object? SolvePart2(string[] lines)
    {
        PrepareMaps(lines, out var sizeX, out var sizeY, out var size, out var rolls, out var counts);

        Queue<int> queue = new Queue<int>(size);


        for (int i = 0; i < size; i++)
        {
            if (rolls[i] && counts[i] < MAX)
                queue.Enqueue(i);
        }

        int removed = 0;

        while (queue.TryDequeue(out var idx))
        {
            if (!rolls[idx])
                continue;
            if (counts[idx] >= MAX)
                continue;

            rolls[idx] = false;
            removed++;

            int y = idx / sizeX;
            int x = idx - y * sizeX;

            for (int d = 0; d < 8; d++)
            {
                int nx = x + dirX[d];
                int ny = y + dirY[d];

                if ((uint)nx >= (uint)sizeX || (uint)ny >= (uint)sizeY)
                    continue;

                int ni = ny * sizeX + nx;
                if (!rolls[ni])
                    continue;

                if (counts[ni] > 0)
                    counts[ni]--;

                if (counts[ni] < MAX)
                    queue.Enqueue(ni);
            }
        }

        return removed;
    }
}
