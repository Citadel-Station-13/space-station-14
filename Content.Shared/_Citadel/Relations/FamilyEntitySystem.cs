// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Robust.Shared.Utility;

namespace Content.Shared._Citadel.Relations;

/// <summary>
///     An interface implementable on EntitySystems to indicate they manage a given family (parent-child) relation.
/// </summary>
public abstract class FamilyEntitySystem<TChild, TParent> : CitadelSystem
    where TChild : IRelationChild, IComponent
    where TParent : IRelationParent, IComponent
{
    /// <summary>
    ///     Event fired when a child is seperated from its parent.
    /// </summary>
    /// <remarks>
    ///     If received from one of the family bulk actions like deletion of the parent,
    ///     the Parent and Children values will never be in a state of partial resolution.
    /// </remarks>
    [PublicAPI]
    public record struct SeperatedEvent(EntityUid? Child, EntityUid? Parent);

    /// <summary>
    ///     Event fired when a child is joined with a parent.
    /// </summary>
    /// <remarks>
    ///     If received from one of the family bulk actions like deletion of the parent,
    ///     the Parent and Children values will never be in a state of partial resolution.
    /// </remarks>
    [PublicAPI]
    public record struct JoinedEvent(EntityUid? Child, EntityUid? Parent);

    protected EntityQuery<TChild> ChildQuery { get; private set; }
    protected EntityQuery<TParent> ParentQuery { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
        if (typeof(TChild) != typeof(TParent))
            SubscribeLocalEvent<TChild, ComponentShutdown>(OnChildShutdown);
        if (typeof(TChild) != typeof(TParent))
            SubscribeLocalEvent<TParent, ComponentShutdown>(OnParentShutdown);
        if (typeof(TChild) == typeof(TParent))
            SubscribeLocalEvent<TChild, ComponentShutdown>(OnMixedShutdown);

        ChildQuery = GetEntityQuery<TChild>();
        ParentQuery = GetEntityQuery<TParent>();
    }

    private void OnMixedShutdown(Entity<TChild> ent, ref ComponentShutdown args)
    {
        OnChildShutdown(ent, ref args);
        OnParentShutdown(new Entity<TParent>(ent, (TParent)(object)ent.Comp), ref args);
    }

    private void OnParentShutdown(Entity<TParent> parent, ref ComponentShutdown args)
    {
        if (parent.Comp.Children.Count == 0)
            return;

        var children = parent.Comp.Children;

        parent.Comp.Children = new HashSet<EntityUid>(); // clear.

        foreach (var child in children)
        {
            var childComp = ChildQuery.Comp(child);
            childComp.Parent = null;

            RaiseLocalEvent(child, new SeperatedEvent(child, parent));
            RaiseLocalEvent(parent, new SeperatedEvent(child, parent));
        }
    }

    private void OnChildShutdown(Entity<TChild> child, ref ComponentShutdown args)
    {
        if (child.Comp.Parent is null)
            return;

        var parent = ParentQuery.Get(child.Comp.Parent.Value);

        // ReSharper disable once RedundantAssignment
        var removed = parent.Comp.Children.Remove(child);

        RaiseLocalEvent(child, new SeperatedEvent(child, parent));
        RaiseLocalEvent(parent, new SeperatedEvent(child, parent));

        DebugTools.Assert(removed);
    }

    /// <summary>
    ///     Makes the given child and parent related, given the child is not already related to another parent.
    /// </summary>
    /// <param name="child">The child in the relation.</param>
    /// <param name="parent">The parent in the relation.</param>
    public void MakeRelated(Entity<TChild> child, Entity<TParent> parent)
    {
        DebugTools.Assert(child.Comp.Parent == null);

        child.Comp.Parent = parent;
        parent.Comp.Children.Add(child);

        RaiseLocalEvent(child, new JoinedEvent(child, parent));
        RaiseLocalEvent(parent, new JoinedEvent(child, parent));
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.MakeRelated(Robust.Shared.GameObjects.Entity{`0},Robust.Shared.GameObjects.Entity{`1})"/>
    public void MakeRelated(EntityUid child, EntityUid parent)
    {
        MakeRelated(ChildQuery.Get(child), ParentQuery.Get(parent));
    }

    /// <summary>
    ///     Attempts to retrieve the parent of a given child.
    /// </summary>
    /// <param name="child">The child to get relation of.</param>
    /// <param name="parent">The found parent, if any.</param>
    /// <returns>Whether a parent was found.</returns>
    public bool TryGetParent(Entity<TChild> child, [NotNullWhen(true)] out EntityUid? parent)
    {
        parent = child.Comp.Parent;

        return child.Comp.Parent.HasValue;
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.TryGetParent(Robust.Shared.GameObjects.Entity{`0},System.Nullable{Robust.Shared.GameObjects.EntityUid}@)"/>
    public bool TryGetParent(EntityUid child, [NotNullWhen(true)] out EntityUid? parent)
    {
        return TryGetParent(ChildQuery.Get(child), out parent);
    }

    /// <summary>
    ///     Retrieves the parent of a given child, or throws.
    /// </summary>
    /// <param name="child">The child to get relation of.</param>
    /// <returns>The parent entity.</returns>
    public EntityUid GetParent(Entity<TChild> child)
    {
        return child.Comp.Parent!.Value;
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.GetParent(Robust.Shared.GameObjects.Entity{`0})"/>
    public EntityUid GetParent(EntityUid child)
    {
        return ChildQuery.Comp(child).Parent!.Value;
    }

    /// <summary>
    ///     Retrieves all children of the given parent.
    /// </summary>
    public IReadOnlySet<EntityUid> GetChildren(Entity<TParent> parent)
    {
        return parent.Comp.Children;
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.GetChildren(Robust.Shared.GameObjects.Entity{`1})"/>
    public IReadOnlySet<EntityUid> GetChildren(EntityUid parent)
    {
        return ParentQuery.Comp(parent).Children;
    }
}
