internal class Program
{
    private static void Main(string[] args)
    {
        Day6();
    }



    private static void Day6()
    {
        var lines = File.ReadAllLines("..\\..\\..\\input.txt");

        var times = lines[0][11..].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => int.Parse(t)).ToArray();
        var ogRecords = lines[1][11..].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => int.Parse(t)).ToArray();

        List<(int, int)> races = new();
        for (int i = 0; i < times.Length; i++) races.Add((times[i], ogRecords[i]));
        
        // for each race, estimate possible wins:
        int[] winPossobiolities = new int[races.Count];

        for (int i = 0; i < races.Count; i++)
        {
            (int, int) race = races[i];
            var time = race.Item1;
            var record = race.Item2;

            // brute force: find all options that will yield distance better than record
            for (int t = 0; t <= time; t++)
            {
                // we keep this time, and convert it to distance that we will travel in remaining time
                var distanceTravelled = (time - t) * t;

                if (distanceTravelled > record)
                {
                    winPossobiolities[i]++;
                }
            }
        }

        var sum = winPossobiolities[0];
        for (int i1 = 1; i1 < winPossobiolities.Length; i1++)
        {
            int w = winPossobiolities[i1];
            sum *= w;
        }

        Console.WriteLine($"Part 1 {sum}");


        // part 2:
        var time2 = long.Parse(lines[0][11..].Replace(" ", ""));
        var record2 = long.Parse(lines[1][11..].Replace(" ", ""));
        long winPossobiolies2 = 0;

        for (int t = 0; t <= time2; t++)
        {
            // we keep this time, and convert it to distance that we will travel in remaining time
            var distanceTravelled = (time2 - t) * t;
            if (distanceTravelled > record2)
                winPossobiolies2++;
        }
        Console.WriteLine($"Part 2 {winPossobiolies2}");
    }




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
    private static void Day5()
    {
        static List<long> GetSeeds(string[] lines)
            => lines[0][7..].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => long.Parse(i)).ToList();
        static List<Seed> GetSeeds2(string[] lines)
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
        static List<Map> GetMap(string[] lines, string name)
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
        static long GetValue(List<Map> maps, long src)
        {
            // look for this value in each map
            // if not found, return src
            // if found, map src to dst
            var map = maps.SingleOrDefault(m => src >= m.Source && src < m.Source + m.Range);
            if (map == null) return src;

            var k = src - map.Source;
            return map.Destination + k;
        }

        var lines = File.ReadAllLines("..\\..\\..\\input.txt");
        var seeds = GetSeeds(lines);
        var seedToSoilMap = GetMap(lines, "seed-to-soil map:");
        var SoilToFertilizerMap = GetMap(lines, "soil-to-fertilizer map:");
        var FertilizerTowaterMap = GetMap(lines, "fertilizer-to-water map:");
        var WaterToLightMap = GetMap(lines, "water-to-light map:");
        var LightToTemperatureMap = GetMap(lines, "light-to-temperature map:");
        var TemperatureToHumidityMap = GetMap(lines, "temperature-to-humidity map:");
        var HumidityToLocationMap = GetMap(lines, "humidity-to-location map:");

        // first approach
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
        Console.WriteLine($"Lowest location number: {lowest}");

        // second problem
        var seeds2 = GetSeeds2(lines);


        var lowestLocNumber = long.MaxValue;
        int sem = 0;
        object consoleSync = new object();
        Console.WriteLine($"{seeds2.Count} seeds to calculate");
        ThreadPool.SetMaxThreads(6, 100);
        foreach (var s2 in seeds2)
        {
            sem++;
            Console.Write($"Calculating seed {s2.Start:N0} - {s2.Start + s2.Range:N0} (Range: {s2.Range:N0}) ");
            var posX = Console.CursorLeft;
            var posY = Console.CursorTop;
            Console.WriteLine();
            ThreadPool.QueueUserWorkItem((o) =>
            {
                CalcSeed2(s2, posX, posY);
            });
            //Thread.Sleep(5000);
            Console.WriteLine();
        }
        while (sem > 0) Thread.Sleep(10);

        Console.WriteLine($"Lowest location number (problem 2): {lowestLocNumber}");

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
                        Console.Write($"{si - s2.Start:N0} ({percenage:F2})");
                    }
                }
            }
            sem--;
        }
    }

    class Card
    {
        public int Number;
        public int Wins;
        public int CardSum;
        public int Copies;
        public override string ToString() => $"Card {Number}, Wins: {Wins}, copies: {Copies}";
    }
    private static void Day4()
    {
        static List<Card> GetOriginalCards(string[] lines)
        {
            List<Card> cards = new();

            for (int i1 = 0; i1 < lines.Length; i1++)
            {
                string? line = lines[i1];
                var sp1 = line.Split(':');
                var cardNum = int.Parse(sp1[0][5..]);
                var sp2 = sp1[1].Split('|');
                var winningStr = sp2[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var currentStr = sp2[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var winning = winningStr.Select(w => int.Parse(w.Trim())).ToList();
                var current = currentStr.Select(w => int.Parse(w.Trim())).ToList();

                var myWins = winning.Where(w => current.Contains(w)).ToList();
                if (myWins.Count == 0)
                {
                    cards.Add(new Card
                    {
                        CardSum = 0,
                        Number = cardNum,
                        Wins = myWins.Count
                    });
                    continue;
                }

                var cardSum = 1; // first point
                for (int i = 1; i < myWins.Count; i++)
                {
                    cardSum *= 2;
                }

                cards.Add(new Card
                {
                    CardSum = cardSum,
                    Number = cardNum,
                    Wins = myWins.Count
                });
            }
            return cards;
        }


        var lines = File.ReadAllLines("..\\..\\..\\input.txt");
        var sum = 0;

        var originalCards = GetOriginalCards(lines);
        sum += originalCards.Sum(c => c.CardSum);

        for (int i = 0; i < originalCards.Count; i++)
        {
            var c = originalCards[i];
            for (int j = 1; j <= c.Wins; j++)
            {
                if (j + i >= originalCards.Count) continue;
                var c2 = originalCards[i + j];
                c2.Copies += 1 + c.Copies;
            }
        }

        // so, for each card, number of wins doubles with each copy


        Console.WriteLine($"Total sum: {sum}");
        Console.WriteLine($"Total cards: {originalCards.Sum(c => 1 + c.Copies)}");
    }

    struct Day3Str1
    {
        public int x;
        public int y;
        public char c;
        public override readonly string ToString()
        {
            return $"{c} - {x}, {y}";
        }
    }
    struct Day3Str2
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;
        public int value;

        public override readonly string ToString()
        {
            return $"({x1}, {y1}), ({x2}, {y2}), ({value})";
        }

    }
    private static void Day3()
    {
        static IEnumerable<Day3Str1> EnumerateSymbols(string[] lines)
        {
            List<Day3Str1> ret = new();
            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    var c = line[x];
                    if (c == '.') continue;
                    if (char.IsDigit(c)) continue;
                    ret.Add(new Day3Str1 { x = x, y = y, c = c });
                }
            }
            return ret;
        }
        static IEnumerable<Day3Str2> EnumerateNumbers(string[] lines)
        {
            List<Day3Str2> ret = new();

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                var numStr = "";
                for (int x = 0; x < line.Length; x++)
                {
                    var c = line[x];
                    if (char.IsDigit(c))
                    {
                        numStr = "";
                        int x2 = 0;
                        for (x2 = x; x2 <= line.Length; x2++)
                        {
                            if (x2 == line.Length)
                            {
                                ret.Add(new Day3Str2
                                {
                                    y1 = y,
                                    y2 = y,
                                    x1 = x,
                                    x2 = x2 - 1,
                                    value = int.Parse(numStr)
                                });
                                break;
                            }
                            var c2 = line[x2];
                            if (!char.IsDigit(c2))
                            {
                                ret.Add(new Day3Str2
                                {
                                    y1 = y,
                                    y2 = y,
                                    x1 = x,
                                    x2 = x2 - 1,
                                    value = int.Parse(numStr)
                                });
                                break;
                            }
                            numStr += c2;
                        }

                        x = x2 - 1;
                    }
                }
            }

            return ret;
        }


        var lines = File.ReadAllLines("..\\..\\..\\input.txt");

        // first, enumerate all symbols, with their (x and y) coords
        var symbols = EnumerateSymbols(lines);

        // then get all numbers, with their values, start x,y and end x, y
        var numbers = EnumerateNumbers(lines);

        // then, for each number, see if there is a symbol in vicinity (1 from number in any direction)
        var sum = 0;
        foreach (var number in numbers)
        {
            foreach (var s in symbols)
            {
                if (s.x < number.x1 - 1) continue;
                if (s.x > number.x2 + 1) continue;

                if (s.y < number.y1 - 1) continue;
                if (s.y > number.y2 + 1) continue;

                sum += number.value;
            }
        }

        var gearRationSum = 0;
        foreach (var s in symbols.Where(sym => sym.c == '*'))
        {
            List<Day3Str2> lst = new();
            foreach (var number in numbers)
            {
                if (s.x < number.x1 - 1) continue;
                if (s.x > number.x2 + 1) continue;

                if (s.y < number.y1 - 1) continue;
                if (s.y > number.y2 + 1) continue;
                lst.Add(number);
            }
            if (lst.Count == 2)
                gearRationSum += lst[0].value * lst[1].value;
        }

        Console.WriteLine($"Parts sum: {sum}");

        Console.WriteLine($"Gear ratio sum: {gearRationSum}");


    }
    private static void Day2()
    {
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

        var lines = File.ReadAllLines("..\\..\\..\\input.txt");

        var possibleGamesSum = 0;
        var gamePowers = 0;
        foreach (var line in lines)
        {
            var sp1 = line.Split(':');
            var gameNum = int.Parse(sp1[0][5..]);

            if (GetMarblesCountForGame(sp1[1]))
            {
                possibleGamesSum += gameNum;
            }

            gamePowers += GetRequiredMarbelsCountForGameMultiplied(sp1[1]);
        }

        Console.WriteLine($"Possible games sum: {possibleGamesSum}");
        Console.WriteLine($"Powers sum: {gamePowers}");
    }
    private static void Day1()
    {
        var lines = File.ReadAllLines("..\\..\\..\\input.txt");
        var sum = 0;
        foreach (var line in lines)
        {
            sum += GetSum(line.ToLower().Trim());
        }
        Console.WriteLine(sum);

        static int GetSum(string line)
        {
            bool GetWordedDigit(string line, int index, string digit)
            {
                if (index + digit.Length > line.Length) return false;

                for (int i = index; i < line.Length; i++)
                {
                    if ((i - index) == digit.Length) break;
                    char c = line[i];
                    var c2 = digit[i - index];
                    if (c != c2) return false;
                }
                return true;
            }

            List<char> list = new();
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (GetWordedDigit(line, i, "one")) { list.Add('1'); /*i += 2;*/ }
                else if (GetWordedDigit(line, i, "two")) { list.Add('2'); /*i += 2;*/ }
                else if (GetWordedDigit(line, i, "three")) { list.Add('3'); /*i += 4;*/ }
                else if (GetWordedDigit(line, i, "four")) { list.Add('4'); /*i += 3; */}
                else if (GetWordedDigit(line, i, "five")) { list.Add('5'); /*i += 3;*/ }
                else if (GetWordedDigit(line, i, "six")) { list.Add('6'); /*i += 2;*/ }
                else if (GetWordedDigit(line, i, "seven")) { list.Add('7'); }
                else if (GetWordedDigit(line, i, "eight")) { list.Add('8'); /*i += 4;*/ }
                else if (GetWordedDigit(line, i, "nine")) { list.Add('9'); }
                else if (c >= '0' && c <= '9') list.Add(c);
            }
            if (list.Count == 0) return 0;

            var num = $"{list[0]}{list[^1]}";
            Console.WriteLine(num);
            return int.Parse(num);
        }

    }

}