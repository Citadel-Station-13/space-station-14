namespace Content.Server._00Citadel.Worldgen.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed class BiomeSelectionComponent : Component
{
    [DataField("biomes", required: true)]
    public List<string> Biomes = default!;
}
