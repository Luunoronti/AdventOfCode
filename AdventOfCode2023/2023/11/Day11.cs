using System.Text;
using static System.Windows.Forms.LinkLabel;

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


        private static void PrintMap(IEnumerable<string> map, string title = "")
        {
            if (Log.Enabled == false) return;

            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(title) == false) sb.AppendLine(title);

            var lastC = CC.Sys;
            foreach (var item in map)
            {
                foreach (var c in item)
                {
                    var clr = CC.Frm;
                    if (c == '#')
                        clr = CC.Att;

                    if (clr != lastC)
                        sb.Append(clr);
                    sb.Append(c);
                    lastC = clr;
                }
                sb.AppendLine();

            }
            sb.AppendLine(CC.Clr);
            Log.WriteLine(sb.ToString());
        }
        private static List<string> InvertMap(string[] lines)
        {
            var nl2 = new List<string>();
            var sb = new StringBuilder();
            for (int j = 0; j < lines[0].Length; j++)
            {
                sb.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    sb.Append(lines[i][j]);
                }
                nl2.Add(sb.ToString());
            }
            return nl2;
        }
        private static int CountEmptyLinesBefore(int x, bool[] data)
        {
            var sum = 0;
            for (int i = 0; i < Math.Min(data.Length, x); i++)
            {
                if (data[i])
                    sum++;
            }
            return sum;
        }
        private static List<Star> GetStarsWithPositions(List<string> map, bool[] emptyRowsMap, bool[] emptyColumnsMap, long expanse = 2)
        {
            var ret = new List<Star>();
            long currY = 0;
            for (int y = 0; y < map.Count; y++)
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
                        emptyColumns[i] = false;
                }
            }
            return emptyColumns;
        }
        public static long Part1(string[] lines)
        {
            var map = new List<string>(lines);
            var mapInverted = InvertMap(lines);

            var emptyRows = GetRowsEmptyFull(lines);
            var emptyColumns = GetColumnsEmptyFull(lines);

            Log.WriteLine("Rows: " + string.Join(" ", emptyRows));
            Log.WriteLine("Cols: " + string.Join(" ", emptyColumns));

            PrintMap(map, "Map");
            PrintMap(mapInverted, "Map inverted");

            var stars = GetStarsWithPositions(map, emptyRows, emptyColumns);
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
            var map = new List<string>(lines);
            var mapInverted = InvertMap(lines);

            PrintMap(map, "Map");
            PrintMap(mapInverted, "Map inverted");

            var emptyRows = GetRowsEmptyFull(lines);
            var emptyColumns = GetColumnsEmptyFull(lines);


            var stars = GetStarsWithPositions(map, emptyRows, emptyColumns, 1000000);

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