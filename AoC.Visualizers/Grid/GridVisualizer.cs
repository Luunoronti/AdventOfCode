//using System.Windows.Forms;
//using Microsoft.VisualStudio.DebuggerVisualizers;
//using AoC.Core;
//using System.Diagnostics;
//using System.Windows.Controls;

//[assembly: DebuggerVisualizer(
//    typeof(AoC.Visualizers.GridVisualizer),
//    typeof(VisualizerObjectSource),
//    Target = typeof(Grid<int>),
//    Description = "Grid Visualizer")]

//namespace AoC.Visualizers;

//public class GridVisualizer : DialogDebuggerVisualizer
//{
//    protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
//    {
//        if (windowService == null || objectProvider == null)
//            return;

//        var grid = (Grid<int>)objectProvider.GetObject();

//        // Simple WinForms UI
//        var form = new Form { Text = "Grid Visualizer", Width = 400, Height = 400 };
//        var textBox = new TextBox
//        {
//            Multiline = true,
//            Dock = DockStyle.Fill,
//            Font = new System.Drawing.Font("Consolas", 12)
//        };

//        for (int y = 0; y < grid.Height; y++)
//        {
//            for (int x = 0; x < grid.Width; x++)
//                textBox.AppendText($"{grid[x, y],3}");
//            textBox.AppendText(Environment.NewLine);
//        }

//        form.Controls.Add(textBox);
//        windowService.ShowDialog(form);
//    }
//}
