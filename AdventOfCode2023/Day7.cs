using System.ComponentModel;

namespace AdventOfCode2023
{
    public static class OtherExtentions
    {
        public static string ToReadable(this int[]? array) => array == null ? "<null>" : $"[{string.Join(", ", array)}]";
        public static List<T> SortRet<T>(this List<T> list, IComparer<T> comparer)
        {
            list.Sort(comparer);
            return list;
        }
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var e in enumerable)
                action?.Invoke(e);
            return enumerable;
        }
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var count = enumerable.Count();
            for (int i = 0; i < count; i++)
                action?.Invoke(enumerable.ElementAt(i), i);
            return enumerable;
        }
    }
    class Day7
    {
        enum HandStr { HighCard, OnePair, TwoPair, ThreeOfKind, FullHouse, FourOfKind, FiveOfKind }
        static readonly List<char> powerMap = new() { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };
        static readonly List<char> powerMapWithJoker = new() { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' };

        class Hand
        {
            const int HandStrLen = 11;
            public class Comparer : IComparer<Hand>
            {
                public bool UseJokerRule { get; set; }
                public bool LowToHigh { get; set; }
                int IComparer<Hand>.Compare(Hand? x, Hand? y)
                {
                    if (x == null) throw new NullReferenceException("x");
                    if (y == null) throw new NullReferenceException("y");

                    var xs = UseJokerRule ? x.HandStrWithJokerRule : x.HandStr;
                    var ys = UseJokerRule ? y.HandStrWithJokerRule : y.HandStr;

                    var sortRet = LowToHigh ? -1 : 1;
                    if (xs != ys)
                        return xs.CompareTo(ys) * sortRet;

                    var xpwrs = UseJokerRule ? x.PowersWithJokerRule : x.Powers;
                    var ypwrs = UseJokerRule ? y.PowersWithJokerRule : y.Powers;
                    for (int i = 0; i < x.Cards.Length; i++)
                    {
                        var c = (xpwrs[i] - ypwrs[i]) * sortRet;
                        if (c != 0) return c;
                    }
                    return 0;
                }
                public static HandStr GetStr(int[] groups)
                {
                    if (groups.Contains(5)) return HandStr.FiveOfKind;
                    if (groups.Contains(4)) return HandStr.FourOfKind;
                    if (groups.Contains(3)) return groups.Contains(2) ? HandStr.FullHouse : HandStr.ThreeOfKind;
                    if (groups.Contains(2)) return groups.Count(g => g == 2) == 2 ? HandStr.TwoPair : HandStr.OnePair;
                    return HandStr.HighCard;
                }
            }
            public Hand(string line)
            {
                Cards = line.SplitAtAsString(' ', 0);
                Bid = line.SplitAtAsInt(' ', 1);
                Groups = Cards.ToCharArray()
                       .GroupBy(x => x)
                       .Select(g => g.Count())
                       .OrderByDescending(x => x)
                       .ToArray();
                if (Cards.All(c => c == 'J'))
                    GroupsWithJokerRule = new int[1] { 5 };
                else
                {
                    GroupsWithJokerRule = Cards.Where(c => c != 'J')
                       .GroupBy(x => x)
                       .Select(g => g.Count())
                       .OrderByDescending(x => x)
                       .ToArray();
                    
                    GroupsWithJokerRule[0] += Cards.Count(c => c == 'J');
                }
                HandStr = GetStr(Groups);
                HandStrWithJokerRule = GetStr(GroupsWithJokerRule);
                Powers = Cards.Select(c => (powerMap.Count - powerMap.IndexOf(c)) + 1).ToArray();
                PowersWithJokerRule = Cards.Select(c => (powerMapWithJoker.Count - powerMapWithJoker.IndexOf(c)) + 1).ToArray();
            }

            public int Bid { get; private set; }
            public int Rank { get; set; }
            public string Cards { get; private set; }
            public int[] Groups { get; private set; }
            public int[] GroupsWithJokerRule { get; private set; }
            public HandStr HandStr { get; private set; }
            public HandStr HandStrWithJokerRule { get; private set; }
            public int[] Powers { get; private set; }
            public int[] PowersWithJokerRule { get; private set; }
            private static HandStr GetStr(int[] groups)
            {
                if (groups.Contains(5)) return HandStr.FiveOfKind;
                if (groups.Contains(4)) return HandStr.FourOfKind;
                if (groups.Contains(3)) return groups.Contains(2) ? HandStr.FullHouse : HandStr.ThreeOfKind;
                if (groups.Contains(2)) return groups.Count(g => g == 2) == 2 ? HandStr.TwoPair : HandStr.OnePair;
                return HandStr.HighCard;
            }
            public string ToString(bool useJokerRule) => useJokerRule ?
                $"{Cards}, Bid: {Bid,4}, Rank: {Rank,4}, Str: {HandStrWithJokerRule,HandStrLen}, Groups: {GroupsWithJokerRule.ToReadable(),15}, Powers: {Powers.ToReadable()}" :
                $"{Cards}, Bid: {Bid,4}, Rank: {Rank,4}, Str: {HandStr,HandStrLen}, Groups: {Groups.ToReadable(),15}, Powers: {Powers.ToReadable()}";
        }


        public static void Run(string[] lines)
        {
            var hands = lines.Select(l => new Hand(l))
                .ToList();
            var sum = hands
                .SortRet(new Hand.Comparer { UseJokerRule = false, LowToHigh = true })
                .ForEach((h, i) => h.Rank = lines.Length - i)
                //.ForEach(h => Console.WriteLine($"Hand: {h.ToString(false)}"))
                .Sum(h => h.Bid * h.Rank);
            Console.WriteLine($"Part 1: {sum}");

            sum = hands
               .SortRet(new Hand.Comparer { UseJokerRule = true, LowToHigh = true })
               .ForEach((h, i) => h.Rank = lines.Length - i)
               .ForEach(h => Console.WriteLine($"Hand: {h.ToString(true)}"))
               .Sum(h => h.Bid * h.Rank);
            Console.WriteLine($"Part 2: {sum}");
        }
    }
}