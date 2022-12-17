using Content.Server._00Citadel.Worldgen.Prototypes;
using Content.Server._00Citadel.Worldgen.Systems;

namespace Content.Server._00Citadel.Worldgen.Components;

/// <summary>
/// This is used for containing configured noise generators.
/// </summary>
[RegisterComponent, Access(typeof(NoiseIndexSystem))]
public sealed class NoiseIndexComponent : Component
{
    public Dictionary<string, NoiseGenerator> Generators { get; } = new();
}
