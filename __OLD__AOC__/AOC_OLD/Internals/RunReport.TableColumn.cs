// attempt to keep the code as clean as possible
// most of functionality is in Core and Extensions folder

partial class RunReport
{
    private class TableColumn
    {
        const string ESC = "\x1b";
        public string Header { get; set; }
        private List<string> Values { get; } = [];
        private List<ConsoleColor> RowColors { get; } = [];
        public int Count => Values.Count;

        public void Add(string value, ConsoleColor color)
        {
            Values.Add(value);
            RowColors.Add(color);
            _maxWidth = -1;
        }
        public int _maxWidth = -1;
        public int MaximumWidth
        {
            get
            {
                if (Values.Count > 0 && _maxWidth == -1) _maxWidth = Values.Max(v => v.Length);
                _maxWidth = Math.Max(_maxWidth, Header.Length);
                return _maxWidth + 2;
            }
        }
        public string ToString(int index)
        {
            if (index == -1)
                return $"{Header.PadLeft(MaximumWidth - 1)} ";
            return $"{ESC}[{CrlCode(RowColors[index])}m{Values[index].PadLeft(MaximumWidth - 1)} {ESC}[0m";
        }

        private int CrlCode(ConsoleColor Color)
        {
            return Color switch
            {
                ConsoleColor.Black => 30,
                ConsoleColor.DarkBlue => 34,
                ConsoleColor.DarkGreen => 32,
                ConsoleColor.DarkCyan => 36,
                ConsoleColor.DarkRed => 31,
                ConsoleColor.DarkMagenta => 35,
                ConsoleColor.DarkYellow => 33,
                ConsoleColor.Gray => 39,
                ConsoleColor.DarkGray => 39,
                ConsoleColor.Blue => 34,
                ConsoleColor.Green => 32,
                ConsoleColor.Cyan => 36,
                ConsoleColor.Red => 31,
                ConsoleColor.Magenta => 35,
                ConsoleColor.Yellow => 33,
                ConsoleColor.White => 37,
                _ => 0,
            };
            /*
            40	Background Black	Applies non-bold/bright black to background
            41	Background Red	Applies non-bold/bright red to background
            42	Background Green	Applies non-bold/bright green to background
            43	Background Yellow	Applies non-bold/bright yellow to background
            44	Background Blue	Applies non-bold/bright blue to background
            45	Background Magenta	Applies non-bold/bright magenta to background
            46	Background Cyan	Applies non-bold/bright cyan to background
            47	Background White	Applies non-bold/bright white to background
            48	Background Extended	Applies extended color value to the background (see details below)
            49	Background Default	Applies only the background portion of the defaults (see 0)
                        */
        }
    }
}
