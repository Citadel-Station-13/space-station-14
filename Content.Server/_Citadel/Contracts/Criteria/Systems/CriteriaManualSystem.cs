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
        args.Info = new CriteriaDisplayData(FormattedMessage.FromMarkup(component.Description));
    }
}
