
namespace Year2025;

class Day03
{
    
    public string Part1(PartInput Input)
    {
        //return Part1F(Input);
        return Input.Lines.Select(line =>
        {
            var allDigits = line.ToCharArray().Select(c => (int)(c - '0')).ToList();
            var m1 = allDigits[..^1].Max();
            var max2 = allDigits[(allDigits.IndexOf(m1) + 1)..].Max();
            return (long)m1 * 10 + max2;
        }).Sum().ToString();
    }
    public string Part2(PartInput Input)
    {
        return Input.Lines.Select(line =>
        {
            // please look for comments bellow as to why line.ToCharArray() is being used
            var allDigits = line.ToCharArray().Select(c => (int)(c - '0')).ToList();
            //var allDigits = line.Select(c => c - '0').ToList();
            
            var startIndex = 0;
            return Enumerable.Range(0, 12).Aggregate(0L, (lineValue, step) =>
            {
                var end = allDigits.Count - (12 - step);
                var sr = allDigits[startIndex..(end + 1)];
                var max = sr.Max();
                var maxIndex = sr.IndexOf(max) + startIndex;
                startIndex = maxIndex + 1;
                return lineValue * 10 + max;
            });
        }).Sum().ToString();
    }





    // Part1() speeds:
    // 100 탎 on test (measurement is faulty, it fluctuates from 0 to 800 ns)
    // 38.5 탎 on live

    // Part2() speeds:
    // 100-400 탎 on test (again, measurement fluctuates)
    // 66.7 탎 on live

    // the method bellow is to test if two for() loops are better than a shallow copy of the List<int>
    // Part1F() speeds:
    // around 200 ns on test
    // 47.4 탎 on live (slower than Part1() (??))

    // so, unless i do something terribly wrong, the method above (Part1()) is not only 
    // way more readable, but also faster
    public string Part1F(PartInput Input)
    {
        return Input.Lines.Select(line =>
        {
            // interesting: line.Select() makes the run time grow to over 80 탎
            // var allDigits = line.Select(c => c - '0').ToList();
            var allDigits = line.ToCharArray().Select(c => (int)(c - '0')).ToList(); // so this one seems to perform better

            // same in Part2(): the line.Select() made that method go over 1 ms on live data
            // while line.ToCharArray().Select() makes it ~66 탎

            int max1Index = 0;
            int m1 = 0;
            int m2 = 0;
            for (int i = 0; i < allDigits.Count - 1; i++)
            {
                var v = allDigits[i];
                if (v > m1)
                {
                    max1Index = i;
                    m1 = v;
                }
            }
            for (int i = max1Index + 1; i < allDigits.Count; i++)
            {
                var v = allDigits[i];
                if (v > m2)
                    m2 = v;
            }
            return (long)m1 * 10 + m2;
        }).Sum().ToString();
    }

}
