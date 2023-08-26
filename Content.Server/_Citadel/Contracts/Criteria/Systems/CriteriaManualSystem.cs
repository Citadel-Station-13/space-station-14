using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Criteria.Components;
using Content.Shared._Citadel.Contracts.BUI;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Contracts.Criteria.Systems;

/// <summary>
/// This handles the admin-triggered manual criteria.
/// </summary>
public sealed partial class CriteriaManualSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<CriteriaManualComponent, CriteriaGetDisplayInfo>(OnGetDisplayInfo);
    }

    private void OnGetDisplayInfo(EntityUid uid, CriteriaManualComponent component, ref CriteriaGetDisplayInfo args)
    {
        var desc = FormattedMessage.FromMarkup(component.Description);
        if (Comp<ContractCriteriaComponent>(uid).Satisfied)
            desc.AddText(" [COMPLETE]");
        args.Info = new CriteriaDisplayData(desc);
    }
}
