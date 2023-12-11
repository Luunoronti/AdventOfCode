namespace AdventOfCode2023
{
    //[Force] // uncomment to force processing this type
    [AlwaysEnableLog]
    //[DisableLogInDebug]
    [UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day11
    {
        class Star
        {
            public long x;
            public long y;
        }
        public static string TestFile => "2023\\11\\test.txt";
        public static string LiveFile => "2023\\11\\live.txt";

        private static List<Star> GetStarsWithPositions(string[] map, bool[] emptyRowsMap, bool[] emptyColumnsMap, long expanse = 2)
        {
            var ret = new List<Star>();
            long currY = 0;
            for (int y = 0; y < map.Length; y++)
            {
                if (emptyRowsMap[y])
                    currY += expanse;
                else
                    currY++;

                long currX = 0;
                for (int x = 0; x < map[0].Length; x++)
                {
                    if (emptyColumnsMap[x])
                        currX += expanse;
                    else
                        currX++;

                    var c = map[y][x];
                    if (c == '#')
                    {
                        var star = new Star { x = currX, y = currY };
                        ret.Add(star);
                        Log.WriteLine($"Star: {star.x}, {star.y}");
                    }
                }
            }
            return ret;
        }

        private static bool[] GetRowsEmptyFull(string[] lines)
        {
            var emptyRows = new bool[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                emptyRows[i] = !lines[i].Any(c => c == '#');
            }
            return emptyRows;
        }
        private static bool[] GetColumnsEmptyFull(string[] lines)
        {
            var emptyColumns = new bool[lines[0].Length];
            for (int i = 0; i < lines[0].Length; i++)
            {
                emptyColumns[i] = true;
                for (int j = 0; j < lines.Length; j++)
                {
                    if (lines[j][i] == '#')
                    {
                        emptyColumns[i] = false;                                   
                        break;
                    }
                }
            }
            return emptyColumns;
        }
        public static long Part1(string[] lines)
        {
            var emptyRows = GetRowsEmptyFull(lines);
            var emptyColumns = GetColumnsEmptyFull(lines);

            var stars = GetStarsWithPositions(lines, emptyRows, emptyColumns);
            var sum = 0L;
            for (int i = 0; i < stars.Count; i++)
            {
                Star star = stars[i];
                for (int i2 = i; i2 < stars.Count; i2++)
                {
                    Star star2 = stars[i2];
                    sum += Math.Abs(star2.x - star.x) + Math.Abs(star2.y - star.y);
                }
            }

            return sum;
        }
        public static long Part2(string[] lines)
        {
            var emptyRows = GetRowsEmptyFull(lines);
            var emptyColumns = GetColumnsEmptyFull(lines);
            var stars = GetStarsWithPositions(lines, emptyRows, emptyColumns, 1000000);

            var sum = 0L;
            for (int i = 0; i < stars.Count; i++)
            {
                Star star = stars[i];
                for (int i2 = i; i2 < stars.Count; i2++)
                {
                    Star star2 = stars[i2];
                    sum += Math.Abs(star2.x - star.x) + Math.Abs(star2.y - star.y);
                }
            }

            return sum;
        }
    }
}