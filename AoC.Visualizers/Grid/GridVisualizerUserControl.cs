using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.UI;
using Newtonsoft.Json.Linq;

namespace AoC.Visualizers;



/// <summary>
/// Remote user control to visualize the <see cref="Match"/> value.
/// </summary>
internal class GridVisualizerUserControl : RemoteUserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridVisualizerUserControl"/> class.
    /// </summary>
    /// <param name="dataContext">Data context of the remote control.</param>
    public GridVisualizerUserControl(RegexMatch dataContext)
        : base(dataContext)
    {
    }
}
