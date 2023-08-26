using Content.Server._Citadel.Contracts.Components;
using Content.Shared._Citadel.Contracts;

namespace Content.Server._Citadel.Contracts.Systems;

// SHITCODESHITCODESHITCODESHITCODESHITCODE
// Whoever touches this next, REPLACE IT.
public sealed class DefaultContractSpawningSystem : EntitySystem
{
    [Dependency] private readonly ContractManagementSystem _contract = default!;

    public Dictionary<string, EntityUid> Contracts = new ()
    {
        {"CitadelMiningVesselContract", EntityUid.Invalid},
        {"CitadelPlasmaMiningContract", EntityUid.Invalid},
    };

    public override void Update(float frameTime)
    {
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
