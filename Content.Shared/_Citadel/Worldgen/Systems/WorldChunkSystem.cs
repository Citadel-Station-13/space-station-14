using Content.Shared._Citadel.Relations;
using Content.Shared._Citadel.Worldgen.Components;

namespace Content.Shared._Citadel.Worldgen.Systems;

/// <summary>
///     Manages world chunks incl their creation, disposal, load states, etc.
/// </summary>
public sealed class WorldChunkSystem : FamilyEntitySystem<CitadelWorldChunkComponent, CitadelWorldChunkManagerComponent>
{
    public override bool ExpensiveRecursionChecks => false; // no need.

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CitadelWorldChunkManagerComponent, SeperatedEvent>(OnSeperated);
    }

    private void OnSeperated(Entity<CitadelWorldChunkManagerComponent> ent, ref SeperatedEvent args)
    {
        // Remove from the index.
        ent.Comp.ChildrenIndex.Remove(args.Child.Comp.Index);
    }
}
