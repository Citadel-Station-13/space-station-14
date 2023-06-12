using System.Linq;
using Content.Server._Citadel.Contracts;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.VesselContracts.Components;
using Content.Server.Chat.Systems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Shared.Map;

namespace Content.Server._Citadel.VesselContracts.Systems;

/// <summary>
/// This handles managing contracted vessels.
/// </summary>
public sealed class ContractVesselManagementSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly ShuttleSystem _shuttle = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ContractSimpleVesselProviderComponent, ContractStatusChangedEvent>(OnContractStatusChanged);
    }

    private void OnContractStatusChanged(EntityUid uid, ContractSimpleVesselProviderComponent component, ContractStatusChangedEvent args)
    {
        if (args.New == ContractStatus.Active && args.Old == ContractStatus.Initiating)
        {
            // failure cases galore, wowee.
            // Syntax is slightly arcane so tl;dr that match is getting the owning entity as a non-nullable EntityUid or else returning.
            if (!TryComp<ContractComponent>(uid, out var contract) || contract.OwningContractor is not { OwnedEntity: { } owner})
                return;

            if (_station.GetOwningStation(owner) is not { } station)
                return; // Need them to be on a station.

            if (_station.GetLargestGrid(Comp<StationDataComponent>(station)) is not { } stationGrid)
                return; // Need the grid so we can dock to it.

            var holding = _map.CreateMap();

            var success = _mapLoader.TryLoad(holding, component.VesselMap, out var roots, new MapLoadOptions()
            {
                LoadMap = false,
            });

            if (!success)
                goto fail; // pain!

            if (roots!.Count != 1)
                goto fail;

            // Can't go fail for this because we can't tell if it failed outright or prox docked. Result<T,E> when?
            // TODO(lunar): Tell sloth to make this return an enum of conditions instead.
            _shuttle.TryFTLDock(roots[0], Comp<ShuttleComponent>(roots[0]), stationGrid);

            Comp<VesselContractComponent>(uid).Vessel = roots[0];

            _chat.DispatchStationAnnouncement(station, $"The vessel for contract {ToPrettyString(uid)} has been docked, if possible.", "Oversight");

            return;

            fail:
            Del(_map.GetMapEntityId(holding));
            return;
        }
    }
}
