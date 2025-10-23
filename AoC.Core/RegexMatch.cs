using System.Runtime.Serialization;

namespace AoC.Visualizers;

[DataContract]
public class RegexMatch : RegexGroup
{
    [DataMember]
    public RegexGroup[]? Groups { get; set; }
}
