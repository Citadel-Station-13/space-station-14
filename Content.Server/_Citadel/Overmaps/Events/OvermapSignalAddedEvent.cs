namespace Content.Server._Citadel.Overmaps.Events;

[ByRefEvent]
public readonly record struct OvermapSignalAddedEvent(OvermapSignal Signal);
