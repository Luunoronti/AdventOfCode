using System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AdventOfCodeUWP
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AoCDayAttribute : Attribute
    {
        public AoCDayAttribute(int year, int day)
        {
            Year = year;
            Day = day;
        }

        public int Year { get; private set; }
        public int Day { get; private set; }
    }

}
