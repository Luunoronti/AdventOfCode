namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    [ExpectedResult("test", 7)]
    [ExpectedResult("live", 571)]
    public static unsafe long SolvePart1(string FilePath)
    {
        var UsedStackMemory = 0;

        var FileSize = FileIO.GetFileSize(FilePath);
        ReadOnlySpan<byte> Buffer = stackalloc byte[(int)FileSize];
        UsedStackMemory += Buffer.Length;
        FileIO.ReadToBuffer(FilePath, Buffer);

        var bufferIndex = 0;
        var total = 0;
        var bufferLen = Buffer.Length;
        Span<int> buttonMasks = stackalloc int[32];
        Span<int> jolts = stackalloc int[16];
        var maxMemPermachine = 0;

        while (true)
        {
            // skip empty chars
            while (bufferIndex < bufferLen)
            {
                var c = Buffer[bufferIndex];
                if (c == (byte)' ' || c == (byte)'\t' || c == (byte)'\r' || c == (byte)'\n') bufferIndex++;
                else break;
            }
            if (bufferIndex >= Buffer.Length) break;

            ParseMachine(Buffer, ref bufferIndex, buttonMasks, jolts, out var buttonCount, out var lightCount, out var targetMask, out var joltsCount);

            var usedMemPermachine = 0;
            var machine = ProcessMachine(lightCount, targetMask, buttonMasks[..buttonCount], ref usedMemPermachine);
            maxMemPermachine = Math.Max(maxMemPermachine, usedMemPermachine);
            total += machine;
        }

        UsedStackMemory += maxMemPermachine;
        // 27385 bytes total
        //Console.WriteLine($"P1 UsedStackMemory: {UsedStackMemory}");

        return total;
    }



    [ExpectedResult("test", 33)]
    [ExpectedResult("live", 20869)]
    public static unsafe long SolvePart2(string FilePath)
    {
        var UsedStackMemory = 0;

        var FileSize = FileIO.GetFileSize(FilePath);
        ReadOnlySpan<byte> Buffer = stackalloc byte[(int)FileSize];
        UsedStackMemory += Buffer.Length;
        FileIO.ReadToBuffer(FilePath, Buffer);

        var bufferIndex = 0;
        var total = 0;
        var bufferLen = Buffer.Length;
        Span<int> buttonMasks = stackalloc int[32];
        Span<int> jolts = stackalloc int[16];
        var maxMemPermachine = 0;

        while (true)
        {
            // skip empty chars
            while (bufferIndex < bufferLen)
            {
                var c = Buffer[bufferIndex];
                if (c == (byte)' ' || c == (byte)'\t' || c == (byte)'\r' || c == (byte)'\n') bufferIndex++;
                else break;
            }
            if (bufferIndex >= Buffer.Length) break;

            ParseMachine(Buffer, ref bufferIndex, buttonMasks, jolts, out var buttonCount, out var lightCount, out var targetMask, out var joltsCount);

            var usedMemPermachine = 0;
            var machine = SolveJoltageMath(buttonMasks[..buttonCount], jolts[..joltsCount], ref usedMemPermachine);
            maxMemPermachine = Math.Max(maxMemPermachine, usedMemPermachine);

            total += machine;
        }
        // 22634 used
        UsedStackMemory += maxMemPermachine;
        //Console.WriteLine($"P2 UsedStackMemory: {UsedStackMemory}");

        return total;
    }


    private static int ProcessMachine(int lightCount, int targetMask, ReadOnlySpan<int> buttonMasks, ref int usedMemory)
    {
        var maxStates = 1 << lightCount;
        Span<byte> visited = stackalloc byte[maxStates];
        Span<short> dist = stackalloc short[maxStates];
        Span<int> queueBuffer = stackalloc int[maxStates];

        usedMemory = maxStates * (1 + 2 + 4); // byte + short + int :)

        var queue = new SpanQueue<int>(queueBuffer);

        var start = 0;
        visited[start] = 1;
        dist[start] = 0;
        queue.Enqueue(start);

        while (queue.TryDequeue(out var state))
        {
            if (state == targetMask) return dist[state];

            var d = dist[state];
            for (var i = 0; i < buttonMasks.Length; i++)
            {
                var next = state ^ buttonMasks[i];
                if (next >= maxStates) continue;
                if (visited[next] != 0) continue;
                visited[next] = 1;
                dist[next] = (short)(d + 1);
                queue.Enqueue(next);
            }
        }

        return 0;
    }


    /*
    https://en.wikipedia.org/wiki/System_of_linear_equations
    https://en.wikipedia.org/wiki/Gaussian_elimination
    https://en.wikipedia.org/wiki/Rank_%28linear_algebra%29
    https://en.wikipedia.org/wiki/Diophantine_equation
    */
    static int SolveJoltageMath(ReadOnlySpan<int> buttonMasks, ReadOnlySpan<int> targets, ref int usedMemory)
    {
        // n = number of counters (size of {...})
        // m = number of buttons (number of () groups)
        var n = targets.Length;
        var m = buttonMasks.Length;
        if (n == 0 || m == 0) return 0;

        // build a linear system A * x = b
        // - A is an n * m matrix with entries 0 or 1, which you can treat
        //   as a table of truth for buttons to counters interaction
        // - x[j] = how many times button j is pressed
        // - b[i] = target value for counter i
        
        // mat holds A in row-major form: mat[i * m + j] = A_ij
        Span<double> mat = stackalloc double[n * m]; usedMemory += n * m * 8;

        // rhs holds the right-hand side vector b
        Span<double> rhs = stackalloc double[n]; usedMemory += n * 8;

        // copy targets into rhs (b)
        for (var i = 0; i < n; i++) rhs[i] = targets[i];

        // build matrix from buttonMasks
        for (var j = 0; j < m; j++)
        {
            var mask = buttonMasks[j];
            for (var i = 0; i < n; i++)
            {
                if (((mask >> i) & 1) != 0)
                    mat[i * m + j] = 1.0;
            }
        }

        // maxX[j] = a safe upper bound on x_j (presses of button j).
        Span<int> maxX = stackalloc int[m]; usedMemory += m * 4;
        for (var j = 0; j < m; j++)
        {
            var mask = buttonMasks[j];
            var mx = int.MaxValue;
            for (var i = 0; i < n; i++)
            {
                if (((mask >> i) & 1) == 0) continue;
                var t = targets[i];
                if (t < mx) mx = t;
            }
            maxX[j] = mx == int.MaxValue ? 0 : mx;
        }

        // rank of the matrix A after Gaussian ellimination
        var rank = 0;

        // which column is the pivot in this row (after elimination)
        // If pivotCol[row] = c, then column c is basic (pivot).
        Span<int> pivotCol = stackalloc int[n]; usedMemory += n * 4;
        for (var i = 0; i < n; i++) pivotCol[i] = -1;

        const double eps = 1e-9;

        // Gaussian elimination on A|b:
        // we are working over doubles because it's easy for elimination,
        // but the original matrix is 0/1 and b is integer.
        for (var col = 0; col < m && rank < n; col++)
        {
            // find the row with the largest |A[row,col]| below current 'rank'
            var bestRow = -1;
            var bestAbs = 0.0;
            for (var row = rank; row < n; row++)
            {
                var v = mat[row * m + col];
                var av = v >= 0 ? v : -v;
                if (av > bestAbs + 1e-12)
                {
                    bestAbs = av;
                    bestRow = row;
                }
            }

            // if there is no non-zero entry in this column, move on to the next column
            if (bestRow == -1) continue;

            // swap the best row with the current 'rank' row (partial pivoting)
            if (bestRow != rank)
            {
                for (var c = col; c < m; c++)
                {
                    var tmp = mat[rank * m + c];
                    mat[rank * m + c] = mat[bestRow * m + c];
                    mat[bestRow * m + c] = tmp;
                }
                var tmpR = rhs[rank];
                rhs[rank] = rhs[bestRow];
                rhs[bestRow] = tmpR;
            }

            // mark this column as pivot column for this row
            pivotCol[rank] = col;
            var pivotVal = mat[rank * m + col];

            // eliminate this column in all rows below
            for (var row = rank + 1; row < n; row++)
            {
                var v = mat[row * m + col];
                if (Math.Abs(v) < eps) continue;
                var factor = v / pivotVal;
                rhs[row] -= factor * rhs[rank];
                for (var c = col; c < m; c++)
                    mat[row * m + c] -= factor * mat[rank * m + c];
            }

            rank++;
        }

        // if a row is all zeros in A but rhs[row] != 0, then no solution exists.
        for (var row = rank; row < n; row++)
        {
            var rowZero = true;
            for (var c = 0; c < m; c++)
            {
                if (Math.Abs(mat[row * m + c]) > eps)
                {
                    rowZero = false;
                    break;
                }
            }
            if (rowZero && Math.Abs(rhs[row]) > eps) return 0;
        }

        // isPivot[col] = 1 if column 'col' is a pivot column (basic variable)
        Span<byte> isPivot = stackalloc byte[m]; usedMemory += m * 1;
        for (var r = 0; r < rank; r++)
        {
            var c = pivotCol[r];
            if (c >= 0) isPivot[c] = 1;
        }

        // freeCols[] = indices of non-pivot columns (free variables)
        Span<int> freeCols = stackalloc int[m]; usedMemory += m * 4;
        var freeCount = 0;
        for (var col = 0; col < m; col++)
        {
            if (isPivot[col] == 0) freeCols[freeCount++] = col;
        }

        // x will hold candidates (as doubles) while we test it
        Span<double> x = stackalloc double[m]; usedMemory += m * 8;
        var bestSum = int.MaxValue;
        var any = false;

        // 1: no free variables, unique solution (over reals)
        if (freeCount == 0)
        {
            // start all x's at 0
            for (var i = 0; i < m; i++) x[i] = 0.0;

            // Back-substitution from last pivot row up to first
            for (var row = rank - 1; row >= 0; row--)
            {
                var colP = pivotCol[row];
                var sum = 0.0;
                for (var c = colP + 1; c < m; c++)
                    sum += mat[row * m + c] * x[c];
                var pivotVal = mat[row * m + colP];
                if (Math.Abs(pivotVal) < eps) return 0;
                var val = (rhs[row] - sum) / pivotVal;
                x[colP] = val;
            }

            // Now we have a real solution. We need it to be:
            // - integer
            // - within 0..maxX[col]
            // and we return the sum of x_j if valid.
            var total = 0;
            for (var col = 0; col < m; col++)
            {
                var val = x[col];
                var r = Math.Round(val);
                if (Math.Abs(val - r) > eps) return 0;
                var xi = (int)r;
                if (xi < 0 || xi > maxX[col]) return 0;
                total += xi;
            }
            return total;
        }

        // 2: there are free variables.
        // enumerate all combinations of integer values for these free variables in 0..maxX[col],
        // and for each, compute the corresponding basic variables via back-substitution.
        // Among all valid integer non-negative solutions, we pick the one with minimal sum of x_j.

        // freeVal[i] = current value for free variable corresponding to freeCols[i]
        Span<int> freeVal = stackalloc int[freeCount]; usedMemory += freeCount * 4;
        for (var i = 0; i < freeCount; i++) freeVal[i] = 0;

        while (true)
        {
            // Initialize x with current free variables
            for (var i = 0; i < m; i++) x[i] = 0.0;
            for (var i = 0; i < freeCount; i++)
            {
                var col = freeCols[i];
                x[col] = freeVal[i];
            }

            // Compute basic variables via back-substitution, given the chosen free variables.
            var bad = false;
            for (var row = rank - 1; row >= 0; row--)
            {
                var colP = pivotCol[row];
                var sum = 0.0;
                for (var c = colP + 1; c < m; c++)
                    sum += mat[row * m + c] * x[c];
                var pivotVal = mat[row * m + colP];
                if (Math.Abs(pivotVal) < eps)
                {
                    bad = true;
                    break;
                }
                var val = (rhs[row] - sum) / pivotVal;

                // We require x[colP] to be integer and within bounds [0..maxX[colP]]
                var r = Math.Round(val);
                if (Math.Abs(val - r) > eps)
                {
                    bad = true;
                    break;
                }
                var xi = (int)r;
                if (xi < 0 || xi > maxX[colP])
                {
                    bad = true;
                    break;
                }
                x[colP] = xi;
            }

            if (!bad)
            {
                // We have a candidate integer solution x[], now compute its total presses.
                var total = 0;
                for (var col = 0; col < m; col++)
                {
                    var v = x[col];
                    var r = Math.Round(v);
                    var xi = (int)r;
                    total += xi;
                }
                // Keep the minimal total over all valid solutions
                if (total < bestSum)
                {
                    bestSum = total;
                    any = true;
                }
            }

            // we treat freeVal as a mixed-radix counter with upper bounds maxX[col].
            var pos = freeCount - 1;
            while (pos >= 0)
            {
                freeVal[pos]++;
                var col = freeCols[pos];
                if (freeVal[pos] <= maxX[col]) break;
                freeVal[pos] = 0;
                pos--;
            }
            if (pos < 0) break;
        }

        // If we found any valid integer solution, return the minimal sum. Otherwise, no solution.
        return any ? bestSum : 0;
    }

    static void ParseMachine(ReadOnlySpan<byte> buffer, ref int index, Span<int> buttonMasks, Span<int> jolts, out int buttonCount, out int lightCount, out int targetMask, out int joltsCount)
    {
        var len = buffer.Length;
        while (index < len && (buffer[index] == (byte)' ' || buffer[index] == (byte)'\t' || buffer[index] == (byte)'\r' || buffer[index] == (byte)'\n')) index++;
        if (index >= len)
        {
            buttonCount = 0;
            lightCount = 0;
            targetMask = 0;
            joltsCount = 0;
            return;
        }
        if (buffer[index] != (byte)'[') throw new InvalidOperationException("Expected '['");
        index++;
        lightCount = 0;
        targetMask = 0;
        while (index < len)
        {
            var c = buffer[index];
            if (c == (byte)']')
            {
                index++;
                break;
            }
            if (c == (byte)'.' || c == (byte)'#')
            {
                if (c == (byte)'#') targetMask |= 1 << lightCount;
                lightCount++;
            }
            index++;
        }
        buttonCount = 0;
        joltsCount = 0;
        while (index < len)
        {
            var c = buffer[index];
            if (c == (byte)'{')
            {
                index++;
                var value = 0;
                var inNumber = false;
                while (index < len)
                {
                    c = buffer[index];
                    if (c >= (byte)'0' && c <= (byte)'9')
                    {
                        inNumber = true;
                        value = value * 10 + (c - (byte)'0');
                        index++;
                        continue;
                    }
                    if (inNumber)
                    {
                        jolts[joltsCount++] = value;
                        value = 0;
                        inNumber = false;
                    }
                    if (c == (byte)'}')
                    {
                        index++;
                        break;
                    }
                    index++;
                }
                while (index < len && buffer[index] != (byte)'\n') index++;
                if (index < len && buffer[index] == (byte)'\n') index++;
                break;
            }
            if (c == (byte)'(')
            {
                index++;
                var mask = 0;
                while (index < len)
                {
                    c = buffer[index];
                    if (c == (byte)')')
                    {
                        index++;
                        break;
                    }
                    if (c == (byte)',' || c == (byte)' ' || c == (byte)'\t')
                    {
                        index++;
                        continue;
                    }
                    var value = 0;
                    while (index < len)
                    {
                        c = buffer[index];
                        if (c < (byte)'0' || c > (byte)'9') break;
                        value = value * 10 + (c - (byte)'0');
                        index++;
                    }
                    mask |= 1 << value;
                }
                buttonMasks[buttonCount++] = mask;
                continue;
            }
            index++;
        }
    }
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

    public bool TryDequeue(out T result)
    {
        if (CountValue == 0)
        {
            result = default;
            return false;
        }
        result = Dequeue();
        return true;
    }

    public T Peek()
    {
        if (CountValue == 0)
            throw new InvalidOperationException("Queue is empty.");

        return Buffer[Head];
    }
}
