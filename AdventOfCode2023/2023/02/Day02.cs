namespace AdventOfCode2023
{
    class Day02
    {
        public static string TestFile => "2023\\02\\test.txt";
        public static string LiveFile => "2023\\02\\live.txt";
        public static bool TestData => true;
        
        public static long Part1(string[] lines)
        {
            var possibleGamesSum = 0;
            foreach (var line in lines)
            {
                var sp1 = line.Split(':');
                var gameNum = int.Parse(sp1[0][5..]);
                if (GetMarblesCountForGame(sp1[1]))
                    possibleGamesSum += gameNum;
            }
            return possibleGamesSum;
        }
        public static long Part2(string[] lines)
        {
            var gamePowers = 0;
            foreach (var line in lines)
            {
                var sp1 = line.Split(':');
                gamePowers += GetRequiredMarbelsCountForGameMultiplied(sp1[1]);
            }
            return gamePowers;
        }
        static bool GetMarblesCountForGame(string gameParts)
        {
            var parts = gameParts.Split(';');
            var red = 0;
            var green = 0;
            var blue = 0;

            foreach (var _part in parts)
            {
                var part = _part.Trim();
                var cs = part.Split(',');
                foreach (var _c in cs)
                {
                    var c = _c.Trim();
                    var ls = c.Split(' ');
                    if (ls[1] == "red" && int.Parse(ls[0]) > 12) return false;
                    if (ls[1] == "green" && int.Parse(ls[0]) > 13) return false;
                    if (ls[1] == "blue" && int.Parse(ls[0]) > 14) return false;
                }
            }
            return (red <= 12 && green <= 13 && blue <= 14);
        }

        static int GetRequiredMarbelsCountForGameMultiplied(string gameParts)
        {
            var parts = gameParts.Split(';');
            var red = 0;
            var green = 0;
            var blue = 0;

            foreach (var _part in parts)
            {
                var part = _part.Trim();
                var cs = part.Split(',');
                foreach (var _c in cs)
                {
                    var c = _c.Trim();
                    var ls = c.Split(' ');
                    if (ls[1] == "red") red = Math.Max(red, int.Parse(ls[0]));
                    if (ls[1] == "green") green = Math.Max(green, int.Parse(ls[0]));
                    if (ls[1] == "blue") blue = Math.Max(blue, int.Parse(ls[0]));
                }
            }
            return red * green * blue;

        }

    }
}