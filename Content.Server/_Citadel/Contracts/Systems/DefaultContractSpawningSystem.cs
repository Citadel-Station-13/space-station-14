using Content.Shared._Citadel.CitVars;
using Content.Server._Citadel.Contracts.Components;
using Content.Shared._Citadel.Contracts;
using Robust.Shared.Configuration;

namespace Content.Server._Citadel.Contracts.Systems;

// SHITCODESHITCODESHITCODESHITCODESHITCODE
// Whoever touches this next, REPLACE IT.

//we cri ; m; -bhijn
public sealed class DefaultContractSpawningSystem : EntitySystem
{
    [Dependency] private readonly ContractManagementSystem _contract = default!;
    [Dependency] protected readonly IConfigurationManager ConfigManager = default!;

    public Dictionary<string, EntityUid> Contracts = new ()
    {
        {"CitadelMiningVesselContract", EntityUid.Invalid},
        {"CitadelPlasmaMiningContract", EntityUid.Invalid},
    };

    public override void Update(float frameTime)
    {
        if (!ConfigManager.GetCVar(CitVars.CitDebugContractSpawning))
            return;

        foreach (var (contract, ent) in Contracts)
        {
            if (Deleted(ent) ||
                Comp<ContractComponent>(ent).Status is not ContractStatus.Initiating and not ContractStatus.Uninitialized)
            {
                Contracts[contract] = _contract.CreateUnboundContract(contract);
            }
        }
    }
}
