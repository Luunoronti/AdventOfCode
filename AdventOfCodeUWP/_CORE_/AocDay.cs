using System.Diagnostics;
using System.Numerics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AdventOfCodeUWP
{
    public abstract class AocDay
    {
        public void Perform(bool useLive, int[] testNumbers)
        {
            if (useLive)
            {

            }

            if (testNumbers != null)
            {
                for (int i = 0; i < testNumbers.Length; i++)
                {

                }
            }


            // remember to catch NotImplementedException, this exception is actually ok
            // do print it, but it's not a app threating error



            var sw = Stopwatch.StartNew();
            var value = Part1();
            sw.Stop();
            sw.Restart();
            value = Part2();
            sw.Stop();
        }

        protected abstract BigInteger Part1();
        protected abstract BigInteger Part2();
    }

}
