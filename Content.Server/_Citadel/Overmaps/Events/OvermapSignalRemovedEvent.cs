namespace Content.Server._Citadel.Overmaps.Events;

[ByRefEvent]
public readonly record struct OvermapSignalRemovedEvent(OvermapSignal Signal);
