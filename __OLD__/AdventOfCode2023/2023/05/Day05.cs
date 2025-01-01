using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode2023
{
    //[Force]
    [AlwaysEnableLog]
    //[DisableLogInDebug]
    [UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day05
    {
        public static string TestFile => "2023\\05\\test.txt";
        public static string LiveFile => "2023\\05\\live.txt";


        private static List<MapRange> GetMap(string[] lines, int startOffset, out int consumedLinesCount)
        {
            consumedLinesCount = 0;
            List<MapRange> og = new();

            for (int i = startOffset; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    break;
                var sp = lines[i].SplitAsArrayOfLongs(' ');
                og.Add(new MapRange { source = sp[1], destination = sp[0], range = sp[2] });
                consumedLinesCount++;
            }

            og.Sort((a, b) => a.source.CompareTo(b.source));

            var currentStart = 0L;
            List<MapRange> ret = new();

            for (int i = 0; i < og.Count; i++)
            {
                var source = og[i].source;
                var destination = og[i].destination;
                var range = og[i].range;

                if (source != currentStart)
                {
                    var ns = currentStart;
                    var ne = source - 1;
                    var nr = ne - ns;
                    ret.Add(new MapRange { source = ns, destination = ns, range = nr });
                    currentStart += nr + 1;
                }
                ret.Add(new MapRange { source = source, destination = destination, range = range });
                currentStart += range;
            }

            ret.Add(new MapRange { source = currentStart, destination = currentStart, range = long.MaxValue - currentStart });

            return ret;

        }
        private static List<List<MapRange>> GetMaps(string[] lines, bool firstMapOnly = false)
        {
            var maps = new List<List<MapRange>>();

            for (int i = 2; i < lines.Length; i++)
            {
                // if line start with nondigit, get map from next one
                if (string.IsNullOrEmpty(lines[i])) continue;
                if (char.IsDigit(lines[i][0]) == false) continue;
                maps.Add(GetMap(lines, i, out var consumed));
                i += consumed;
            }

            Log.WriteLine($"Found {CC.Val}{maps.Count}{CC.Clr} maps.");
            if (Log.Enabled)
            {
                foreach (var ml in maps)
                {
                    Log.CreateDataTable("Src", "Dst", "Rng");
                    foreach (var r in ml)
                        Log.AddTableRow(r.source, r.destination, r.range);
                    Log.PrintTable();
                }
            }

            return maps;
        }
        [DisableLogInDebug]
        public static long Part1(string[] lines)
        {
            var seeds = lines[0][7..].SplitAsArrayOfLongs(' ');
            var maps = GetMaps(lines);

            Log.WriteLine($"\nSeeds: {seeds.ToReadable()}");
            var minimum = long.MaxValue;
            foreach (var seed in seeds)
            {
                var intermediate = seed;
                List<long> hst = new();
                hst.Add(intermediate);
                foreach (var map in maps)
                {
                    var range = map.SingleOrDefault(m => intermediate >= m.source && intermediate < m.source + m.range);
                    intermediate = range.destination + (intermediate - range.source);
                    hst.Add(intermediate);
                }
                minimum = Math.Min(minimum, intermediate);

                Log.WriteLine(hst.ToReadable(" => "));
            }

            return minimum;
        }



        struct MapRange
        {
            public long source;
            public long destination;
            public long range;
        }
        class RangeC
        {
            public long Start;
            public long Range;
            public long End
            {
                get => Start + Range;
                set => Range = value - Start;
            }
        }
        public static long Part2(string[] lines)
        {
            var seeds = lines[0][7..].SplitAsArrayOfLongs(' ');
            var seedRanged = new List<RangeC>();
            for (int i = 0; i < seeds.Length; i += 2)
                seedRanged.Add(new RangeC { Start = seeds[i], Range = seeds[i + 1] });
            Log.WriteLine($"Found {CC.Val}{seedRanged.Count}{CC.Clr} seed ranges.");

            var maps = GetMaps(lines, true);

            List<(long, long)> intermediateSeeds = new();


            var seed = seedRanged[0];

            // operate on first map
            var map = maps[0];

            var seedStart = seed.Start;
            var seedEnd = seed.End;

            //while (seedStart < seedEnd)
            //{
            //    // find range
            //    var mapped = GetRange(seedStart, map);

            //    intermediateSeeds.Add((Map(seedStart, mapped), seedStart + mapped.range));
            //    seedStart += mapped.range;
            //}


            long Map(long position, MapRange map) => map.destination + (position - map.source);
            MapRange GetRange(long position, List<MapRange?> map)
            {
                var r = map.SingleOrDefault(m => position >= m?.source && position < m?.source + m?.range);
                if (r != null) return r.Value;

                // construct new range that start at our position, and ends at next map, or at long.maxvalue
                MapRange? found = null;
                foreach (var rr in map)
                {
                    var rrv = rr.Value;
                    if (rrv.source < position) continue;
                    found = rrv;
                }

                return new MapRange { source = position, destination = position, range = found.HasValue ? found.Value.source : long.MaxValue };
            }



            var minimum = long.MaxValue;
            foreach (var seedRange in seedRanged)
            {

            }

            return minimum;


            (long result, long offset) MapFirstAndGetStep(long start, long end)
            {
                // we can map start right away
                var startMapped = start;
                var newEnd = end;
                foreach (var map in maps)
                {
                    var r = map.SingleOrDefault(m => startMapped >= m.source && startMapped < m.source + m.range);
                    startMapped = r.destination + (startMapped - r.source);

                    var off = startMapped - r.destination;
                    newEnd = Math.Min(newEnd, (r.destination + r.range) - off);
                }
                if (newEnd == 0) newEnd = 0; //test only
                return (startMapped, (end) - start);
            }
        }


    }
    class old_Day05
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