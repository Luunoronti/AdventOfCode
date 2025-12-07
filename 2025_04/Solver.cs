namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    const byte MAX = 4;

    static int[] dirX = { -1, 0, 1, -1, 1, -1, 0, 1 };
    static int[] dirY = { -1, -1, -1, 0, 0, 1, 1, 1 };


    public static void PrepareMaps(string[] Lines, int SizeX, int SizeY, int Size, Span<bool> Rolls, Span<byte> Counts)
    {
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

    [ExpectedResult("test", 13)]
    [ExpectedResult("live", 1457)]
    public static long SolvePart1(string[] lines)
    {
        var sizeY = lines.Length;
        var sizeX = lines[0].Length;
        var size = sizeY * sizeX;

        Span<bool> rolls = stackalloc bool[size];
        Span<byte> counts = stackalloc byte[size];

        PrepareMaps(lines, sizeX, sizeY, size, rolls, counts);

        long count = 0;
        for (int k = 0; k < size; k++)
        {
            if (rolls[k] && counts[k] < MAX)
                count++;
        }
        return count;
    }

    [ExpectedResult("test", 43)]
    [ExpectedResult("live", 8310)]
    public static long SolvePart2(string[] lines)
    {
        var sizeY = lines.Length;
        var sizeX = lines[0].Length;
        var size = sizeY * sizeX;

        Span<bool> rolls = stackalloc bool[size];
        Span<byte> counts = stackalloc byte[size];

        PrepareMaps(lines, sizeX, sizeY, size, rolls, counts);

        Span<int> buffer = stackalloc int[size];
        var queue = new SpanQueue<int>(buffer);


        for (int i = 0; i < size; i++)
        {
            if (rolls[i] && counts[i] < MAX)
                queue.Enqueue(i);
        }

        int removed = 0;

        while (!queue.IsEmpty)
        {
            var idx = queue.Dequeue();
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



    public ref struct SpanQueue<T>
    {
        private Span<T> Buffer;
        private int Head;
        private int Tail;
        private int CountValue;

        public SpanQueue(Span<T> buffer)
        {
            Buffer = buffer;
            Head = 0;
            Tail = 0;
            CountValue = 0;
        }

        public int Count => CountValue;
        public bool IsEmpty => CountValue == 0;
        public int Capacity => Buffer.Length;

        public void Enqueue(T value)
        {
            if (CountValue == Buffer.Length)
                throw new InvalidOperationException("Queue is full.");

            Buffer[Tail] = value;
            Tail++;
            if (Tail == Buffer.Length) Tail = 0;
            CountValue++;
        }

        public T Dequeue()
        {
            if (CountValue == 0)
                throw new InvalidOperationException("Queue is empty.");

            var result = Buffer[Head];
            Head++;
            if (Head == Buffer.Length) Head = 0;
            CountValue--;
            return result;
        }

        public T Peek()
        {
            if (CountValue == 0)
                throw new InvalidOperationException("Queue is empty.");

            return Buffer[Head];
        }
    }

}
