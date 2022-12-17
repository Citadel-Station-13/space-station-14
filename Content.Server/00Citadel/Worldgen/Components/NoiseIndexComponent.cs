using Content.Server._00OuterRim.Worldgen2.Prototypes;
using Content.Server._00OuterRim.Worldgen2.Systems;

namespace Content.Server._00OuterRim.Worldgen2.Components;

/// <summary>
/// This is used for containing configured noise generators.
/// </summary>
[RegisterComponent, Access(typeof(NoiseIndexSystem))]
public sealed class NoiseIndexComponent : Component
{
    public Dictionary<string, NoiseGenerator> Generators { get; } = new();
}
