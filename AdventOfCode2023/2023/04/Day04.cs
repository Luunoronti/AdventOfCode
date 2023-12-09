namespace AdventOfCode2023
{
    class Day04
    {
        public static string TestFile => "2023\\04\\test.txt";
        public static string LiveFile => "2023\\04\\live.txt";
        class Card
        {
            public int Number;
            public int Wins;
            public int CardSum;
            public int Copies;
            public override string ToString() => $"Card {Number}, Wins: {Wins}, copies: {Copies}";
        }

        private static List<Card> GetOriginalCards(string[] lines)
        {
            List<Card> cards = new();
            for (int i1 = 0; i1 < lines.Length; i1++)
            {
                string? line = lines[i1];
                var sp1 = line.Split(':');
                var cardNum = int.Parse(sp1[0][5..]);
                var sp2 = sp1[1].Split('|');

                var winning = sp2[0].SplitAsArrayOfLongs(' ');
                var current = sp2[1].SplitAsArrayOfLongs(' ');

                var myWins = winning.Where(w => current.Contains(w)).ToList();
                if (myWins.Count == 0)
                {
                    cards.Add(new Card { CardSum = 0, Number = cardNum, Wins = myWins.Count });
                    continue;
                }

                var cardSum = 1; // first point
                for (int i = 1; i < myWins.Count; i++)
                    cardSum *= 2;

                cards.Add(new Card { CardSum = cardSum, Number = cardNum, Wins = myWins.Count });
            }
            return cards;
        }


        public static bool TestData => true;
        
        public static long Part1(string[] lines)
        {
            var sum = 0L;
            var originalCards = GetOriginalCards(lines);
            sum += originalCards.Sum(c => c.CardSum);
            return sum;
        }
        public static long Part2(string[] lines)
        {
            var originalCards = GetOriginalCards(lines);
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
            return originalCards.Sum(c => 1 + c.Copies);
        }
    }
}