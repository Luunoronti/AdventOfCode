namespace AdventOfCode2023
{
    //[Force]
    class Day05
    {
        public static string TestFile => "2023\\05\\test.txt";
        public static string LiveFile => "2023\\05\\live.txt";
        class Map
        {
            public long Source;
            public long Destination;
            public long Range;
        }
        class Seed
        {
            public long Start;
            public long Range;
        }

        private static List<long> GetSeeds(string[] lines)
           => lines[0][7..].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => long.Parse(i)).ToList();
        private static List<Seed> GetSeeds2(string[] lines)
        {
            var ret = new List<Seed>();
            var sp = lines[0][7..].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sp.Length; i += 2)
            {
                var start = long.Parse(sp[i]);
                var range = long.Parse(sp[i + 1]);
                ret.Add(new Seed { Start = start, Range = range });
            }
            return ret;
        }
        private static List<Map> GetMap(string[] lines, string name)
        {
            List<Map> ret = new();
            for (long i1 = 0; i1 < lines.Length; i1++)
            {
                if (lines[i1].ToLower().StartsWith(name))
                {
                    for (long i = i1 + 1; i < lines.Length; i++)
                    {
                        if (string.IsNullOrEmpty(lines[i])) break;
                        var sp = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        long dstRngStart = long.Parse(sp[0]);
                        long srcRangeStart = long.Parse(sp[1]);
                        long range = long.Parse(sp[2]);

                        ret.Add(new Map { Destination = dstRngStart, Range = range, Source = srcRangeStart });
                    }
                }
            }
            return ret;
        }
        private static long GetValue(List<Map> maps, long src)
        {
            // look for this value in each map
            // if not found, return src
            // if found, map src to dst
            var map = maps.SingleOrDefault(m => src >= m.Source && src < m.Source + m.Range);
            if (map == null) return src;

            var k = src - map.Source;
            return map.Destination + k;
        }

        public static bool TestData => true;
        
        public static long Part1(string[] lines)
        {
            var seeds = GetSeeds(lines);
            var seedToSoilMap = GetMap(lines, "seed-to-soil map:");
            var SoilToFertilizerMap = GetMap(lines, "soil-to-fertilizer map:");
            var FertilizerTowaterMap = GetMap(lines, "fertilizer-to-water map:");
            var WaterToLightMap = GetMap(lines, "water-to-light map:");
            var LightToTemperatureMap = GetMap(lines, "light-to-temperature map:");
            var TemperatureToHumidityMap = GetMap(lines, "temperature-to-humidity map:");
            var HumidityToLocationMap = GetMap(lines, "humidity-to-location map:");

            var locations = seeds.Select(s =>
                GetValue(HumidityToLocationMap,
                GetValue(TemperatureToHumidityMap,
                GetValue(LightToTemperatureMap,
                GetValue(WaterToLightMap,
                GetValue(FertilizerTowaterMap,
                GetValue(SoilToFertilizerMap,
                GetValue(seedToSoilMap, s)))))))
                ).ToList();
            var lowest = locations.Min();
            return lowest;
        }
        public static long Part2(string[] lines)
        {
            var seeds = GetSeeds(lines);
            var seedToSoilMap = GetMap(lines, "seed-to-soil map:");
            var SoilToFertilizerMap = GetMap(lines, "soil-to-fertilizer map:");
            var FertilizerTowaterMap = GetMap(lines, "fertilizer-to-water map:");
            var WaterToLightMap = GetMap(lines, "water-to-light map:");
            var LightToTemperatureMap = GetMap(lines, "light-to-temperature map:");
            var TemperatureToHumidityMap = GetMap(lines, "temperature-to-humidity map:");
            var HumidityToLocationMap = GetMap(lines, "humidity-to-location map:");

            var seeds2 = GetSeeds2(lines);
            var lowestLocNumber = long.MaxValue;
            int sem = 0;
            object consoleSync = new object();
            Log.WriteLine($"{seeds2.Count} seeds to calculate");

            foreach (var s2 in seeds2)
            {
                sem++;
                Log.Write($"Calculating seed {s2.Start:N0} - {s2.Start + s2.Range:N0} (Range: {s2.Range:N0}) ");
                var posX = Console.CursorLeft;
                var posY = Console.CursorTop;
                Log.WriteLine();
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    CalcSeed2(s2, posX, posY);
                });
                Log.WriteLine();
            }
            while (sem > 0) Thread.Sleep(10);

            return lowestLocNumber;
            void CalcSeed2(Seed s2, int consoleX, int consoleY)
            {
                for (long si = s2.Start; si < s2.Start + s2.Range; si++)
                {
                    var locationNumber = GetValue(HumidityToLocationMap,
                        GetValue(TemperatureToHumidityMap,
                        GetValue(LightToTemperatureMap,
                        GetValue(WaterToLightMap,
                        GetValue(FertilizerTowaterMap,
                        GetValue(SoilToFertilizerMap,
                        GetValue(seedToSoilMap, si)))))));

                    lowestLocNumber = Math.Min(lowestLocNumber, locationNumber);

                    if (si % 100000 == 0)
                    {
                        lock (consoleSync)
                        {
                            Console.CursorLeft = consoleX;
                            Console.CursorTop = consoleY;
                            var percenage = (double)(si - s2.Start) / (double)s2.Range;
                            percenage *= 100;
                            Log.Write($"{si - s2.Start:N0} ({percenage:F2})");
                        }
                    }
                }
                sem--;
            }
        }
    }
}