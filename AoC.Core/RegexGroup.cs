using System.Runtime.Serialization;

namespace AoC.Visualizers;

[DataContract]
public class RegexGroup : RegexCapture
{
    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public RegexCapture[]? Captures { get; set; }
}
