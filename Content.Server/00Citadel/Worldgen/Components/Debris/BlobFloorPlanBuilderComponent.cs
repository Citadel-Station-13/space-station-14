using Content.Server._00Citadel.Worldgen.Tools;
using Content.Shared.Maps;
using Content.Shared.Storage;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server._00Citadel.Worldgen.Components.Debris;

/// <summary>
/// This is used for constructing asteroid debris.
/// </summary>
[RegisterComponent]
public sealed class BlobFloorPlanBuilderComponent : Component
{
    [DataField("floorTileset", required: true, customTypeSerializer: typeof(PrototypeIdListSerializer<ContentTileDefinition>))]
    public List<string> FloorTileset { get; } = default!;

    /// <summary>
    /// The number of floor tiles to place when drawing the asteroid layout.
    /// </summary>
    [DataField("floorPlacements", required: true)]
    public int FloorPlacements { get; }

    /// <summary>
    /// The probability that placing a floor tile will add up to three-four neighboring tiles as well.
    /// </summary>
    [DataField("blobDrawProb")]
    public float BlobDrawProb = 0.0f;

    /// <summary>
    /// The maximum radius for the structure.
    /// </summary>
    [DataField("radius", required: true)]
    public float Radius = 0.0f;
}
