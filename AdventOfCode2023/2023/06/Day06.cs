namespace AdventOfCode2023
{
    //[Force]
    class Day06
    {
        public static string TestFile => "2023\\06\\test.txt";
        public static string LiveFile => "2023\\06\\live.txt";
        public static bool TestData => true;
        
        public static long Part1(string[] lines)
        {
            var times = lines[0][11..].SplitAsArrayOfLongs(' ');
            var ogRecords = lines[1][11..].SplitAsArrayOfLongs(' ');

            List<(long, long)> races = new();
            for (int i = 0; i < times.Length; i++) races.Add((times[i], ogRecords[i]));
            int[] winPossobiolities = new int[races.Count];

            for (int i = 0; i < races.Count; i++)
            {
                (long, long) race = races[i];
                var time = race.Item1;
                var record = race.Item2;

                for (int t = 0; t <= time; t++)
                {
                    if ((time - t) * t > record)
                        winPossobiolities[i]++;
                }
            }

            var sum = winPossobiolities[0];
            for (int i1 = 1; i1 < winPossobiolities.Length; i1++)
                sum *= winPossobiolities[i1];

            return sum;
        }
        public static long Part2(string[] lines)
        {
            var time2 = long.Parse(lines[0][11..].Replace(" ", ""));
            var record2 = long.Parse(lines[1][11..].Replace(" ", ""));
            long winPossobilies2 = 0;

            for (int t = 0; t <= time2; t++)
            {
                // we keep this time, and convert it to distance that we will travel in remaining time
                var distanceTravelled = (time2 - t) * t;
                if (distanceTravelled > record2)
                    winPossobilies2++;
            }
            return winPossobilies2;
        }
    }
}