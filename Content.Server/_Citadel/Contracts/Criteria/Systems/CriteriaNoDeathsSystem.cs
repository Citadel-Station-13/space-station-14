using System.Linq;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Criteria.Components;
using Content.Server._Citadel.Contracts.Systems;
using Content.Shared._Citadel.Contracts.BUI;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Contracts.Criteria.Systems;

/// <summary>
/// This handles the "don't die" criteria.
/// </summary>
public sealed partial class CriteriaNoDeathsSystem : EntitySystem
{
    [Dependency] private readonly ContractCriteriaSystem _criteria = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<CriteriaNoDeathsComponent, CriteriaStartTickingEvent>(OnStartTicking);
        SubscribeLocalEvent<CriteriaNoDeathsComponent, CriteriaGetDisplayInfo>(OnGetDisplayInfo);
    }

    private void OnGetDisplayInfo(EntityUid uid, CriteriaNoDeathsComponent component, ref CriteriaGetDisplayInfo args)
    {
        args.Info = new CriteriaDisplayData(FormattedMessage.FromMarkup(component.Description));
    }

    private void OnStartTicking(EntityUid uid, CriteriaNoDeathsComponent component, CriteriaStartTickingEvent args)
    {
        component.Ticking = true;
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<CriteriaNoDeathsComponent, ContractCriteriaComponent>();
        var contractQuery = GetEntityQuery<ContractComponent>();

        while (query.MoveNext(out var uid, out var noDeaths, out var criteria))
        {
            if (!noDeaths.Ticking)
                continue;

            var contract = contractQuery.GetComponent(criteria.OwningContract);

            var anyDead = false;

            foreach (var mind in contract.SubContractors.Append(contract.OwningContractor))
            {
                anyDead |= mind is not null && mind.TimeOfDeath is not null;
            }

            _criteria.SetCriteriaStatus(uid, anyDead, criteria);
        }
    }
}
