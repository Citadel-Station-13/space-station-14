using Content.Server._Citadel.Contracts;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Systems;
using Content.Server._Citadel.MiningContracts.Criteria.Components;
using Content.Server._Citadel.VesselContracts.Components;
using Content.Server._Citadel.VesselContracts.Systems;
using Content.Server.Materials;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared._Citadel.Contracts.BUI;
using Content.Shared.Materials;
using Content.Shared.Mind;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.MiningContracts.Criteria.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class CriteriaMaterialStorageHasMaterialsSystem : EntitySystem
{
    [Dependency] private readonly ContractCriteriaSystem _criteria = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly MaterialStorageSystem _material = default!;
    [Dependency] private readonly ContractVesselManagementSystem _contractVessel = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<CriteriaMaterialStorageHasMaterialsComponent, CriteriaGetDisplayInfo>(OnGetDisplayInfo);
        SubscribeLocalEvent<CriteriaMaterialStorageHasMaterialsComponent, CriteriaStartTickingEvent>(OnStartTicking);
        SubscribeLocalEvent<CriteriaMaterialStorageHasMaterialsComponent, ComponentStartup>(OnComponentStartup);
    }

    private void OnComponentStartup(EntityUid uid, CriteriaMaterialStorageHasMaterialsComponent component, ComponentStartup args)
    {
        component.Amount = _random.Next(component.AmountRange.X, component.AmountRange.Y);
    }

    private void OnGetDisplayInfo(EntityUid uid, CriteriaMaterialStorageHasMaterialsComponent component, ref CriteriaGetDisplayInfo args)
    {
        var desc = FormattedMessage.FromMarkup(
            Loc.GetString("criteria-material-storage-has-materials-component-display-description",
                ("material", component.Material),
                ("amount", component.Amount))
        );

        if (Comp<ContractCriteriaComponent>(uid).Satisfied)
            desc.AddText(" [COMPLETE]");

        args.Info = new CriteriaDisplayData(desc);
    }

    private void OnStartTicking(EntityUid uid, CriteriaMaterialStorageHasMaterialsComponent component, CriteriaStartTickingEvent args)
    {
        component.Ticking = true;
    }

    public override void Update(float frameTime)
    {
        var materialStorageQuery = GetEntityQuery<MaterialStorageComponent>();
        var vesselContractQuery = GetEntityQuery<VesselContractComponent>();
        var q = EntityQueryEnumerator<ContractCriteriaComponent, CriteriaMaterialStorageHasMaterialsComponent>();

        while (q.MoveNext(out var uid, out var cc, out var criteria))
        {
            if (!_entityManager.TryGetComponent<ContractComponent>(cc.OwningContract, out var owningContractComponent))
                continue;

            if (owningContractComponent.OwningContractor is not {} owningContractor)
                continue;

            if (_contractVessel.LocateUserVesselContract(new Entity<MindComponent>(owningContractor.Owner, owningContractor)) is not
                { } vesselUid)
                continue;

            var vessel = vesselContractQuery.GetComponent(vesselUid);

            if (criteria.Ticking is false || criteria.Satisfied || vessel.Vessel is null)
                continue;

            var gridMaybe = _station.GetLargestGrid(Comp<StationDataComponent>(vessel.Vessel!.Value));

            if (gridMaybe is not {} grid)
                continue;

            var storageQuery = EntityQueryEnumerator<ValidMaterialStorageCriteriaMarkerComponent, MaterialStorageComponent, TransformComponent>();

            var matTotal = 0;
            var holders = new List<EntityUid>();

            while (storageQuery.MoveNext(out var sUid, out _, out var storage, out var xform))
            {
                if (xform.GridUid != grid)
                    continue;

                matTotal += _material.GetMaterialAmount(sUid, criteria.Material, storage);
                holders.Add(sUid);
            }

            if (matTotal < criteria.Amount)
                continue;

            foreach (var store in holders)
            {
                var comp = materialStorageQuery.GetComponent(store);

                var amountInStore = _material.GetMaterialAmount(store, criteria.Material, comp);

                if (amountInStore >= matTotal)
                {
                    _material.TryChangeMaterialAmount(store, criteria.Material, -matTotal, comp);
                    break;
                }
                else
                {
                    _material.TryChangeMaterialAmount(store, criteria.Material, -amountInStore, comp);
                    matTotal -= amountInStore;
                }
            }

            criteria.Satisfied = true;
            _criteria.SetCriteriaStatus(uid, true, cc);
        }
    }
}
