using Content.Server._00Citadel.Worldgen.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._00Citadel.Worldgen.Components.GC;

/// <summary>
/// This is used for whether or not a GCable object is "dirty". Firing GCDirtyEvent on the object is the correct way to set this up.
/// </summary>
[RegisterComponent]
public sealed class GCAbleObjectComponent : Component
{
    [DataField("queue", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<GCQueuePrototype>))]
    public string Queue = default!;
}
