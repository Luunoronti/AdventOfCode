using System.IO;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram;

namespace Day24_2024_Graph;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.Loaded += MainWindow_Loaded;
    }

    private NodeViewModel CreateModel(string id, string shape)
    {
        return new NodeViewModel()
        {
            ID = id,
            UnitWidth = 120,
            UnitHeight = 40,
            OffsetX = 300,
            OffsetY = 60,
            Shape = this.Resources[shape],
            ShapeStyle = this.Resources["ShapeStyle"] as Style,
        };
    }
    private ConnectorViewModel CreateConnector(string id1, string id2)
    {
        return new ConnectorViewModel()
        {
            SourceNodeID = id1,
            TargetNodeID = id2,
            TargetDecoratorStyle = this.Resources["TargetDecoratorStyle"] as Style,
            ConnectorGeometryStyle = this.Resources["ConnectorGeometryStyle"] as Style
        };

    }
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // load file with nodes
        var lines = File.ReadAllLines(@"C:\Work\AoC\AdventOfCode\AdventOfCode2024\Test\2024_24_1.txt");
        lines = lines.Where(l => string.IsNullOrEmpty(l) == false).Where(l => l.Contains(":") == false).ToArray();

        var nodes = new NodeCollection();
        var connectors = new ConnectorCollection();


        Dictionary<string, NodeViewModel> nodesD = [];

        int nodeId = 1;
        foreach (var line in lines)
        {
            var sp = line.Split(' ');
            var con1 = sp[0];
            var con2 = sp[2];
            var type = sp[1];
            var resu = sp[4];


            // check if node of con1 exists. if not, create
            if (nodesD.TryGetValue(con1, out var con1n) == false)
                con1n = nodesD[con1] = CreateModel(con1, "Ellipse");

            if (nodesD.TryGetValue(con2, out var con2n) == false)
                con2n = nodesD[con2] = CreateModel(con2, "Ellipse");

            if (nodesD.TryGetValue(resu, out var resun) == false)
                resun = nodesD[resu] = CreateModel(resu, "Ellipse");

            var gateId = nodeId.ToString();
            switch (type)
            {
                case "AND": nodes.Add(CreateModel(gateId, "Process")); break;
                case "OR": nodes.Add(CreateModel(gateId, "PredefinedProcess")); break;
                case "XOR": nodes.Add(CreateModel(gateId, "Process")); break;
            }

            // join
            connectors.Add(CreateConnector(con1, gateId));
            connectors.Add(CreateConnector(con2, gateId));
            connectors.Add(CreateConnector(gateId, resu));


            nodeId++;
        }

        diagram.Nodes = nodes;
        diagram.Connectors = connectors;
    }
}