using System.Data;
using System.Text;

static class Log
{
    public static bool Enabled { get; set; }
    public static void WriteLine(string message) { if (Enabled) Console.WriteLine(message); }
    public static void WriteLine() { if (Enabled) Console.WriteLine(); }
    public static void Write(string message) { if (Enabled) Console.Write(message); }

    private static DataTable? _dataTable;
    public static void CreateDataTable(params string[] columns)
    {
        if (!Enabled) return;
        _dataTable = new DataTable();
        foreach (string column in columns)
            _dataTable.Columns.Add(column);
    }
    public static void AddTableRow(params object[] objects) => _dataTable?.Rows.Add(objects);
    public static void PrintTable()
    {
        if (!Enabled) return;
        if (_dataTable == null) return;
        var widths = new int[_dataTable.Columns.Count];
        int index = 0;
        foreach (DataColumn c in _dataTable.Columns)
        {
            widths[index] = c.ColumnName.Length;
            foreach (DataRow row in _dataTable.Rows)
            {
                var len = row[c].ToString()?.Length ?? 0;
                widths[index] = Math.Max(len, widths[index]);

            }
            index++;
        }
        StringBuilder sb = new();

        sb.Clear();
        index = 0;
        sb.Append($"{CC.Frm}┌");
        foreach (DataColumn c in _dataTable.Columns)
        {
            var w = widths[index];
            sb.Append($"─".PadRight(w + 2, '─'));
            sb.Append($"┬");
            index++;
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append($"┐{CC.Clr}\n");


        index = 0;
        sb.Append($"{CC.Frm}│{CC.Clr}");
        foreach (DataColumn c in _dataTable.Columns)
        {
            var w = widths[index];
            sb.Append($" {c.ColumnName.PadRight(w)} {CC.Frm}│{CC.Clr}");
            index++;
        }
        sb.Append($"\n");

        index = 0;
        sb.Append($"{CC.Frm}├");
        foreach (DataColumn c in _dataTable.Columns)
        {
            var w = widths[index];
            sb.Append($"─".PadRight(w + 2, '─'));
            sb.Append($"┼");
            index++;
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append($"┤{CC.Clr}");
        sb.Append($"\n");

        var rowIndex = 0;
        foreach (DataRow row in _dataTable.Rows)
        {
            index = 0;
            sb.Append($"{CC.Frm}│{CC.Clr}");

            if (rowIndex % 2 != 0)
                sb.Append($"{CC.ABg}");

            foreach (DataColumn c in _dataTable.Columns)
            {
                var w = widths[index];
                if (index == _dataTable.Columns.Count - 1)
                    sb.Append($" {row[c].ToString()?.PadRight(w)} {CC.Clr}{CC.Frm}│");
                else sb.Append($" {row[c].ToString()?.PadRight(w)} {CC.Frm}│{CC.Clr}");

                if (rowIndex % 2 != 0)
                    sb.Append($"{CC.ABg}");

                index++;
            }
            sb.Append($"{CC.Clr}");
            rowIndex++;
            if (index % 2 != 0)
                sb.Append($"{CC.Frm}");
            sb.Append($"\n");
        }

        index = 0;
        sb.Append($"{CC.Frm}└");
        foreach (DataColumn c in _dataTable.Columns)
        {
            var w = widths[index];
            sb.Append($"─".PadRight(w + 2, '─'));
            sb.Append($"┴");
            index++;
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append($"┘{CC.Clr}");
        Console.WriteLine(sb);

    }


    public static string PrintEnumerableWithPadding<T>(int padding, IEnumerable<T> enumerable, bool padLeft = true, string delimiter = "")
    {
        var sb = new StringBuilder();
        var index = 0;
        foreach (T item in enumerable)
        {
            if (padLeft) sb.Append(item?.ToString()?.PadLeft(padding));
            else sb.Append(item?.ToString()?.PadRight(padding));
            if (string.IsNullOrEmpty(delimiter) == false && index != enumerable.Count() - 1)
                sb.Append(delimiter);
            index++;
        }
        return sb.ToString();
    }

}
