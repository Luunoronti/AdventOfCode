using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace AoC.Visualizers;
[VisualStudioContribution]
internal class RegexMatchVisualizerExtension : Extension
{
    /// <inheritdoc/>
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "RegexMatchVisualizer.29d15448-6b97-42e5-97c7-bb12ded13b89",
                version: this.ExtensionAssemblyVersion,
                publisherName: "Microsoft",
                displayName: "Regex Match Debugger Visualizer",
                description: "A debugger visualizer for regex Match and MatchCollection"),
    };

    /// <inheritdoc/>
    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection);

        // You can configure dependency injection here by adding services to the serviceCollection.
    }
}