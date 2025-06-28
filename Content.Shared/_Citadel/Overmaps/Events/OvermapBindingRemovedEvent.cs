using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps.Events;

public sealed class OvermapBindingRemovedEvent : EntityEventArgs
{
    public readonly MapId MapId;
}
