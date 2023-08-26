using System.Linq;
using Content.Server._Citadel.Contracts.Components;
using Content.Shared._Citadel.Contracts;
using Content.Shared._Citadel.Contracts.BUI;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Contracts.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class FinalizeContractsCriteriaSystem : EntitySystem
{
    [Dependency] private readonly ContractCriteriaSystem _criteria = default!;


    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ContractStatusChangedEvent>(Handler);
        SubscribeLocalEvent<FinalizeContractsCriteriaComponent, CriteriaGetDisplayInfo>(OnGetDisplayInfo);
    }

    private void OnGetDisplayInfo(EntityUid uid, FinalizeContractsCriteriaComponent component, ref CriteriaGetDisplayInfo args)
    {
        var desc = FormattedMessage.FromMarkup($"Complete {component.ContractsToClear} contracts successfully while this one is active.");
        if (Comp<ContractCriteriaComponent>(uid).Satisfied)
            desc.AddText(" [COMPLETE]");
        args.Info = new CriteriaDisplayData(desc);
    }

    private void Handler(ContractStatusChangedEvent ev)
    {
        var contractComp = Comp<ContractComponent>(ev.Contract);

        if (ev.New != ContractStatus.Finalized)
            return;

        foreach (var contractor in contractComp.SubContractors.Append(contractComp.OwningContractor))
        {
            var contracts = contractor!.Contracts.SelectMany(x => Comp<ContractCriteriaControlComponent>(x).Criteria.Values)
                .SelectMany(x => x)
                .Where(HasComp<FinalizeContractsCriteriaComponent>);

            foreach (var contract in contracts)
            {
                var finalize = Comp<FinalizeContractsCriteriaComponent>(contract);
                finalize.ContractsCleared++;
                if (finalize.ContractsCleared >= finalize.ContractsToClear)
                    _criteria.SetCriteriaStatus(contract, true, Comp<ContractCriteriaComponent>(contract));
            }
        }
    }
}
