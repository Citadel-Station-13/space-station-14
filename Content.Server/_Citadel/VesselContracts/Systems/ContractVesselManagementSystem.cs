using System.Linq;
using Content.Server._Citadel.Contracts;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.VesselContracts.Components;
using Content.Server.Chat.Systems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Localizations;
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
        SubscribeLocalEvent<ContractSimpleVesselRemoverComponent, ContractStatusChangedEvent>(OnContractStatusChanged);
        SubscribeLocalEvent<VesselContractComponent, ComponentShutdown>(OnVesselContractShutdown);
    }

    private void OnVesselContractShutdown(EntityUid uid, VesselContractComponent component, ComponentShutdown args)
    {
        _station.DeleteStation(component.Vessel!.Value);
    }

    private void OnContractStatusChanged(EntityUid uid, ContractSimpleVesselRemoverComponent component, ContractStatusChangedEvent args)
    {
        if (args is {Old: ContractStatus.Active, New: ContractStatus.Breached or ContractStatus.Finalized})
        {
            component.Active = true;
        }
    }

    public void ContractVesselAnnouncement(EntityUid contract, string message)
    {
        if (!TryComp<VesselContractComponent>(contract, out var vessel))
        {
            throw new ArgumentException("Given entity was not a vessel contract.", nameof(contract));
        }

        _chat.DispatchStationAnnouncement(vessel.Vessel!.Value, message, sender: "Oversight");
    }

    private void OnContractStatusChanged(EntityUid uid, ContractSimpleVesselProviderComponent component, ContractStatusChangedEvent args)
    {
        switch (args)
        {
            case {Old: ContractStatus.Initiating, New: ContractStatus.Active}:
            {
                HandleContractActivation(uid, component);
                return;
            }
            case {Old: ContractStatus.Active, New: ContractStatus.Finalized}:
            {
                ContractVesselAnnouncement(uid, $"Contract #{(int)uid} \"{Name(uid)}\" has been finalized and payment will be processed. Return to your check-in point.");
                return;
            }
            case {Old: ContractStatus.Active, New: ContractStatus.Breached}:
            {
                ContractVesselAnnouncement(uid, $"You have breached the terms of contract #{(int)uid} \"{Name(uid)}\". Return to your check-in point immediately.");
                return;
            }
        }
    }

    private void HandleContractActivation(EntityUid uid, ContractSimpleVesselProviderComponent component)
    {
        // failure cases galore, wowee.
        // Syntax is slightly arcane so tl;dr that match is getting the owning entity as a non-nullable EntityUid or else returning.
        if (!TryComp<ContractComponent>(uid, out var contract) ||
            contract.OwningContractor is not {OwnedEntity: { } owner})
            return;

        if (_station.GetOwningStation(owner) is not { } station)
            return;

        if (_station.GetLargestGrid(Comp<StationDataComponent>(station)) is not { } stationGrid)
            return;

        var holding = _map.CreateMap();

        var success = _mapLoader.TryLoad(holding, component.VesselMap, out var roots, new MapLoadOptions
        {
            LoadMap = false,
        });

        if (!success)
            goto fail; // pain!

        if (roots!.Count != 1)
            goto fail;

        var vessel = _station.InitializeNewStation(component.VesselConfig, roots);

        AddComp<ContractedVesselComponent>(vessel).Contract = uid;

        Comp<VesselContractComponent>(uid).Vessel = vessel;

        var shuttle = _station.GetLargestGrid(Comp<StationDataComponent>(vessel))!.Value;

        // Can't go fail for this because we can't tell if it failed outright or prox docked. Result<T,E> when?
        // TODO(lunar): Tell sloth to make this return an enum of conditions instead.
        _shuttle.TryFTLDock(shuttle, Comp<ShuttleComponent>(shuttle), stationGrid);

        _chat.DispatchStationAnnouncement(station,
            $"The vessel for contract #{(int)uid} \"{Name(uid)}\" has been docked. {ContentLocalizationManager.FormatList(contract.SubContractors.Prepend(contract.OwningContractor!).Select(x => x.CharacterName!).ToList())} should report to their vessel.", "Oversight");

        fail:
        Del(_map.GetMapEntityId(holding));
        return;
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<ContractComponent, VesselContractComponent, ContractSimpleVesselRemoverComponent>();

        while (query.MoveNext(out var uid, out var contract, out var vessel, out var comp))
        {
            if (!comp.Active)
                continue;

            var data = Comp<StationDataComponent>(vessel.Vessel!.Value);

            var nearby = _station.GetInStation(data, range: 10.0f);

            if (!nearby.Recipients.Any())
            {
                foreach (var grid in data.Grids)
                {
                    _station.RemoveGridFromStation(vessel.Vessel!.Value, grid, null, data);
                    QueueDel(grid);
                }
            }
        }
    }
}
