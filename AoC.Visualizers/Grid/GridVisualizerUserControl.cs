using System.Runtime.Serialization;
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





[DataContract]
internal class RegexMatch : RegexGroup
{
    [DataMember]
    public RegexGroup[]? Groups { get; set; }
}

[DataContract]
internal class RegexGroup : RegexCapture
{
    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public RegexCapture[]? Captures { get; set; }
}

[DataContract]
internal class RegexCapture
{
    [DataMember]
    public int Index { get; set; }

    [DataMember]
    public int Length { get; set; }

    [DataMember]
    public string? Value { get; set; }

    [DataMember]
    public string? Name { get; set; }

#if VISUALIZER
    [DataMember]
    public string Range => $"{this.Index}-{this.Index + this.Length}";
#endif
}