using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server._Citadel.Overmaps.Components;
using Content.Server._Citadel.Overmaps.Events;
using Content.Shared._Citadel.Overmaps.Systems;

namespace Content.Server._Citadel.Overmaps.Systems;

/// <inheritdoc/>
public sealed class OvermapEmissionsSystem : SharedOvermapEmissionsSystem
{
    private EntityQuery<OvermapEmissionsComponent> _overmapEmissionsQuery;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        _overmapEmissionsQuery = GetEntityQuery<OvermapEmissionsComponent>();
    }

    public bool TryGetEntitySignals([NotNullWhen(true)] EntityUid entity,
        [NotNullWhen(true)] out IEnumerable<OvermapSignal> signals)
    {
        if (_overmapEmissionsQuery.TryGetComponent(entity, out var emissions))
        {
            signals = emissions.Signals;
            return true;
        }

        signals = [];
        return true;
    }

    public bool AddSignalToEntity(EntityUid entity, OvermapSignal signal)
    {
        if (_overmapEmissionsQuery.TryGetComponent(entity, out var emissions))
        {
            if (emissions.Signals.Contains(signal))
            {
                return true;
            }

            emissions.Signals.Add(signal);
            var sig = new OvermapSignalAddedEvent(signal);
            RaiseLocalEvent(entity, ref sig);
            return true;
        }

        return false;
    }

    public bool RemoveSignalFromEntity(EntityUid entity, OvermapSignal signal)
    {
        if (_overmapEmissionsQuery.TryGetComponent(entity, out var emissions))
        {
            if (!emissions.Signals.Contains(signal))
            {
                return true;
            }

            emissions.Signals.Remove(signal);
            var sig = new OvermapSignalRemovedEvent(signal);
            RaiseLocalEvent(entity, ref sig);
        }

        return true;
    }

    // TODO: standard signal queries
}
