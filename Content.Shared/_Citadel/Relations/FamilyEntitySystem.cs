// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Robust.Shared.Utility;

namespace Content.Shared._Citadel.Relations;

/// <summary>
///     An interface implementable on EntitySystems to indicate they manage a given family (parent-child) relation.
/// </summary>
/// <remarks>
///     This base class has a non-empty initialize and as such base.Initialize() must be called.
/// </remarks>
public abstract partial class FamilyEntitySystem<TChild, TParent> : CitadelSystem
    where TChild : IFamilyRelationChild, IComponent, new()
    where TParent : IFamilyRelationParent, IComponent, new()
{
    /// <summary>
    ///     Whether (relatively expensive) recursion checks should be performed at every relationship form.
    /// </summary>
    public abstract bool ExpensiveRecursionChecks { get; }

    /// <summary>
    ///     Whether to disallow having both TChild and TParent on the same entity.
    /// </summary>
    public virtual bool DisallowBothComponents => false;

    /// <summary>
    ///     Event fired when a child is seperated from its parent.
    /// </summary>
    /// <remarks>
    ///     If received from one of the family bulk actions like deletion of the parent,
    ///     the Parent and Children values will never be in a state of partial resolution.
    /// </remarks>
    [PublicAPI]
    public record struct SeperatedEvent(Entity<TChild> Child, Entity<TParent> Parent);

    /// <summary>
    ///     Event fired when a child is joined with a parent.
    /// </summary>
    /// <remarks>
    ///     If received from one of the family bulk actions like deletion of the parent,
    ///     the Parent and Children values will never be in a state of partial resolution.
    /// </remarks>
    [PublicAPI]
    public record struct JoinedEvent(Entity<TChild> Child, Entity<TParent> Parent);

    [PublicAPI]
    protected EntityQuery<TChild> ChildQuery { get; private set; }
    [PublicAPI]
    protected EntityQuery<TParent> ParentQuery { get; private set; }

    public override void Initialize()
    {
        base.Initialize();

        if (typeof(TChild) != typeof(TParent))
        {
            SubscribeLocalEvent<TChild, ComponentShutdown>(OnChildShutdown);
            SubscribeLocalEvent<TParent, ComponentShutdown>(OnParentShutdown);

            SubscribeLocalEvent<TChild, ComponentStartup>(OnChildStartup);
            SubscribeLocalEvent<TParent, ComponentStartup>(OnParentStartup);
        }
        else if (typeof(TChild) == typeof(TParent))
        {
            SubscribeLocalEvent<TChild, ComponentShutdown>(OnMixedShutdown);

            SubscribeLocalEvent<TChild, ComponentStartup>(OnMixedStartup);
        }

        if (DisallowBothComponents)
            DebugTools.Assert(typeof(TChild) != typeof(TParent), "Disallowing both components when they're the same component is nonsensical.");

        ChildQuery = GetEntityQuery<TChild>();
        ParentQuery = GetEntityQuery<TParent>();
    }

    /// <summary>
    ///     Event handler for child startup.
    /// </summary>
    /// <remarks>
    ///     This should run before your own startup code.
    /// </remarks>
    protected virtual void OnChildStartup(EntityUid uid, TChild component, ref ComponentStartup args)
    {
        if (DisallowBothComponents && HasComp<TParent>(uid))
            throw new MixedParentChildDisallowedException(typeof(TChild), typeof(TParent));
    }

    /// <summary>
    ///     Event handler for parent startup.
    /// </summary>
    /// <remarks>
    ///     This should run before your own startup code.
    /// </remarks>
    protected virtual void OnParentStartup(EntityUid uid, TParent component, ref ComponentStartup args)
    {
        if (DisallowBothComponents && HasComp<TChild>(uid))
            throw new MixedParentChildDisallowedException(typeof(TChild), typeof(TParent));
    }

    /// <summary>
    ///     Event handler for mixed (TParent == TChild) startup.
    /// </summary>
    /// <remarks>
    ///     This should run before your own startup code.
    /// </remarks>
    protected virtual void OnMixedStartup(EntityUid uid, TChild component, ref ComponentStartup args)
    {
        // .. do nothing, yet.
    }

    /// <summary>
    ///     Event handler for mixed (TChild == TParent) shutdown.
    /// </summary>
    /// <remarks>
    ///     This should run before your own shutdown code.
    /// </remarks>
    protected virtual void OnMixedShutdown(Entity<TChild> ent, ref ComponentShutdown args)
    {
        OnChildShutdown(ent, ref args);
        OnParentShutdown(new Entity<TParent>(ent, (TParent)(object)ent.Comp), ref args);
    }

    /// <summary>
    ///     Event handler for parent shutdown.
    /// </summary>
    /// <remarks>
    ///     This should run before your own shutdown code.
    /// </remarks>
    protected virtual void OnParentShutdown(Entity<TParent> parent, ref ComponentShutdown args)
    {
        if (parent.Comp.Children.Count == 0)
            return;

        var children = parent.Comp.Children;

        parent.Comp.Children = new HashSet<EntityUid>(); // clear.

        foreach (var child in children)
        {
            var childComp = ChildQuery.Comp(child);
            childComp.Parent = null;

            RaiseLocalEvent(child, new SeperatedEvent((child, childComp), parent));
            RaiseLocalEvent(parent, new SeperatedEvent((child, childComp), parent));
        }
    }

    /// <summary>
    ///     Event handler for child shutdown.
    /// </summary>
    /// <remarks>
    ///     This should run before your own shutdown code.
    /// </remarks>
    protected virtual void OnChildShutdown(Entity<TChild> child, ref ComponentShutdown args)
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
    [PublicAPI]
    public void MakeRelated(Entity<TChild> child, Entity<TParent> parent)
    {
        DebugTools.Assert(child.Comp.Parent == null);

        if (ExpensiveRecursionChecks)
        {
            if (GetParents(parent).Contains(child))
                ThrowRecursionCheck(parent);
        }

        child.Comp.Parent = parent;
        parent.Comp.Children.Add(child);

        RaiseLocalEvent(child, new JoinedEvent(child, parent));
        RaiseLocalEvent(parent, new JoinedEvent(child, parent));
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.MakeRelated(Robust.Shared.GameObjects.Entity{`0},Robust.Shared.GameObjects.Entity{`1})"/>
    [PublicAPI]
    public void MakeRelated(EntityUid child, EntityUid parent)
    {
        MakeRelated(ChildQuery.Get(child), ParentQuery.Get(parent));
    }

    /// <summary>
    ///     Attempts to remove a relation between the given parent and child.
    /// </summary>
    [PublicAPI]
    [MustUseReturnValue]
    public bool TryRemoveRelation(Entity<TChild> child, Entity<TParent> parent)
    {
        if (child.Comp.Parent != parent)
            return false;

        child.Comp.Parent = null;

        // ReSharper disable once RedundantAssignment
        var success = parent.Comp.Children.Remove(child);
        DebugTools.Assert(success, "Relations broke :(");

        RaiseLocalEvent(child, new SeperatedEvent(child, parent));
        RaiseLocalEvent(parent, new SeperatedEvent(child, parent));

        return true;
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.TryRemoveRelation(Robust.Shared.GameObjects.Entity{`0},Robust.Shared.GameObjects.Entity{`1})"/>
    [PublicAPI]
    [MustUseReturnValue]
    public bool TryRemoveRelation(EntityUid child, EntityUid parent)
    {
        return TryRemoveRelation(ChildQuery.Get(child), ParentQuery.Get(parent));
    }

    /// <summary>
    ///     Removes a relation between the parent and child, throwing otherwise.
    /// </summary>
    /// <exception cref="UnrelatedException">Thrown if the parent and child are unrelated.</exception>
    [PublicAPI]
    public void RemoveRelation(Entity<TChild> child, Entity<TParent> parent)
    {
        if (!TryRemoveRelation(child, parent))
            throw new UnrelatedException(child, parent);
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.RemoveRelation(Robust.Shared.GameObjects.Entity{`0},Robust.Shared.GameObjects.Entity{`1})"/>
    [PublicAPI]
    public void RemoveRelation(EntityUid child, EntityUid parent)
    {
        if (!TryRemoveRelation(ChildQuery.Get(child), ParentQuery.Get(parent)))
            throw new UnrelatedException(child, parent);
    }

    // small helper function to avoid having exception machinery in a relatively critical path.
    private static void ThrowRecursionCheck(EntityUid recursingParent)
    {
        throw new RecursionCheckedException(recursingParent);
    }
}
