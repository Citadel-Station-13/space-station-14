using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps.Events;

public sealed class OvermapBindingAddedEvent : EntityEventArgs
{
    public readonly MapId MapId;
}
