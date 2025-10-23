using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

namespace AoC.Visualizers;

using System.Text.RegularExpressions;

[VisualStudioContribution]
internal class GridVisualizerProvider : DebuggerVisualizerProvider
{
    public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new("AoC Grid Debugger Display", typeof(Match))
    {
        VisualizerObjectSourceType = new(typeof(RegexMatchObjectSource)),
    };


    /// <inheritdoc/>
    public override async Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
    {
        //string targetObjectValue = await visualizerTarget.ObjectSource.RequestDataAsync<string>(jsonSerializer: null, cancellationToken);
        var regexMatch = await visualizerTarget.ObjectSource.RequestDataAsync<RegexMatch>(jsonSerializer: null, cancellationToken);
        return new GridVisualizerUserControl(regexMatch);
    }
}
