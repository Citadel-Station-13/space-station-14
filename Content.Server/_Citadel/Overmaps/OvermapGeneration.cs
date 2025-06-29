using Content.Shared._Citadel.Overmaps.Prototypes;

namespace Content.Server._Citadel.Overmaps;

public sealed class OvermapGeneration
{
    public static readonly OvermapGeneration DoNothing = new OvermapGeneration();

    public List<OvermapLayer> Layers { get; set; } = new List<OvermapLayer>();

    public static OvermapGeneration fromPrototype(OvermapGenerationPrototype prototype)
    {

    }
}
