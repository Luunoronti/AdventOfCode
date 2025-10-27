// attempt to keep the code as clean as possible
// most of functionality is in Core and Extensions folder


using System.Text;

partial class RunReport
{
    private class DayEntry
    {
        public int Day;
        public int Year;
        public string Name;
        public int RowIndex;
    }
    private class Table
    {
        private List<TableColumn> Columns { get; } = [];
        private Dictionary<string, TableColumn> NamedColumns { get; } = [];

        private List<DayEntry> Entries = [];

        public void AddColumn(string name)
        {
            var col = new TableColumn { Header = name };
            Columns.Add(col);
            NamedColumns.Add(name, col);
        }

        public void AddValue(string column, string value, ConsoleColor Color = ConsoleColor.White)
        {
            if (NamedColumns.TryGetValue(column, out TableColumn col))
            {
                col.Add(value, Color);
            }
        }

        public void AddDayEntry(int Day, int Year, string Name)
        {
            Entries.Add(new DayEntry { Day = Day, Year = Year, Name = Name, RowIndex = Columns[0].Count });
        }



        private void PrintHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("┌");
            foreach (var col in Columns)
            {
                sb.Append("".PadLeft(col.MaximumWidth, '─'));
                sb.Append(col == Columns.Last() ? "┐" : "┬");
            }
            Console.WriteLine(sb.ToString());

            sb.Clear();
            sb.Append("│");
            foreach (var col in Columns)
            {
                sb.Append(col.ToString(-1));
                sb.Append("│");
            }
            Console.WriteLine(sb.ToString());

            sb.Clear();
            sb.Append("├");
            foreach (var col in Columns)
            {
                sb.Append("".PadLeft(col.MaximumWidth, '─'));
                sb.Append(col == Columns.Last() ? "┤" : "┴");
            }
            Console.WriteLine(sb.ToString());
        }
        private void PrintRow(int Row)
        {
            StringBuilder sb = new();

            var e = Entries.SingleOrDefault(e => e.RowIndex == Row);
            if (e != null)
            {
                int totalWidth = 0;
                sb.Append("├");
                foreach (var col in Columns)
                {
                    sb.Append("".PadLeft(col.MaximumWidth, '─'));
                    sb.Append(col == Columns.Last() ? "┤" : "─");
                    totalWidth += col.MaximumWidth + 1;
                }
                if (Row > 0)
                    Console.WriteLine(sb.ToString());
                totalWidth -= 1;

                sb.Clear();
                sb.Append("│");
                sb.Append($"{e.Day} / {e.Year} - {e.Name}".PadRight(totalWidth, ' '));

                sb.Append("│");
                Console.WriteLine(sb.ToString());


                sb.Clear();
                sb.Append("├");
                foreach (var col in Columns)
                {
                    sb.Append("".PadLeft(col.MaximumWidth, '─'));
                    sb.Append(col == Columns.Last() ? "┤" : "─");
                }
                Console.WriteLine(sb.ToString());
            }


            sb.Clear();
            sb.Append("│");
            foreach (var col in Columns)
            {
                sb.Append(col.ToString(Row));
                sb.Append("│");
            }
            Console.WriteLine(sb.ToString());
            sb.Clear();
        }
        private void PrintFooter()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("└");
            foreach (var col in Columns)
            {
                sb.Append("".PadLeft(col.MaximumWidth, '─'));
                sb.Append(col == Columns.Last() ? "┘" : "┴");
            }
            Console.WriteLine(sb.ToString());
        }
        public void Print()
        {
            PrintHeader();
            for (int i = 0; i < Columns[0].Count; i++)
                PrintRow(i);
            PrintFooter();
        }

        internal void Format()
        {
        }
    }
}
