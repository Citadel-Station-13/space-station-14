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

            RaiseLocalEvent(child, new SeperatedEvent((child, childComp), parent));
            RaiseLocalEvent(parent, new SeperatedEvent((child, childComp), parent));
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
    ///     Ensures the given entity can be a child.
    /// </summary>
    public void EnsureAbleChild(Entity<TChild?> child)
    {
        EnsureComp<TChild>(child);
    }

    /// <summary>
    ///     Ensures the given entity can be a child, fixing up the <see cref="Entity{T}"/> given.
    /// </summary>
    public void EnsureAbleChild(ref Entity<TChild?> child)
    {
        var t = EnsureComp<TChild>(child);
        child.Comp = t;
    }

    /// <summary>
    ///     Ensures the given entity can be a parent.
    /// </summary>
    public void EnsureAbleParent(Entity<TParent?> parent)
    {
        EnsureComp<TParent>(parent);
    }

    /// <summary>
    ///     Ensures the given entity can be a parent, fixing up the <see cref="Entity{T}"/> given.
    /// </summary>
    public void EnsureAbleParent(ref Entity<TParent?> parent)
    {
        var t = EnsureComp<TParent>(parent);
        parent.Comp = t;
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

    /// <summary>
    ///     Attempts to retrieve the parent of a given child.
    /// </summary>
    /// <param name="child">The child to get relation of.</param>
    /// <param name="parent">The found parent, if any.</param>
    /// <returns>Whether a parent was found.</returns>
    [PublicAPI]
    [Pure]
    public bool TryGetParent(Entity<TChild> child, [NotNullWhen(true)] out EntityUid? parent)
    {
        parent = child.Comp.Parent;

        return child.Comp.Parent.HasValue;
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.TryGetParent(Robust.Shared.GameObjects.Entity{`0},System.Nullable{Robust.Shared.GameObjects.EntityUid}@)"/>
    [PublicAPI]
    [Pure]
    public bool TryGetParent(EntityUid child, [NotNullWhen(true)] out EntityUid? parent)
    {
        return TryGetParent(ChildQuery.Get(child), out parent);
    }

    /// <summary>
    ///     Retrieves the parent of a given child, or throws.
    /// </summary>
    /// <param name="child">The child to get relation of.</param>
    /// <returns>The parent entity.</returns>
    [PublicAPI]
    [Pure]
    public EntityUid GetParent(Entity<TChild> child)
    {
        return child.Comp.Parent!.Value;
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.GetParent(Robust.Shared.GameObjects.Entity{`0})"/>
    [PublicAPI]
    [Pure]
    public EntityUid GetParent(EntityUid child)
    {
        return ChildQuery.Comp(child).Parent!.Value;
    }

    /// <summary>
    ///     Retrieves all children of the given parent.
    /// </summary>
    [PublicAPI]
    [Pure]
    public IReadOnlySet<EntityUid> GetChildren(Entity<TParent> parent)
    {
        return parent.Comp.Children;
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.GetChildren(Robust.Shared.GameObjects.Entity{`1})"/>
    [PublicAPI]
    [Pure]
    public IReadOnlySet<EntityUid> GetChildren(EntityUid parent)
    {
        return ParentQuery.Comp(parent).Children;
    }

    /// <summary>
    ///     Gets all children in a tree, alongside their parent.
    /// </summary>
    /// <exception cref="RecursionCheckedException">If the tree recurses (is a DAG, not a tree), this is thrown.</exception>
    /// <remarks>
    ///     This function is recursive over maxDepth, so maxDepth should be relatively low to avoid out of stackspace scenarios.
    /// </remarks>
    [PublicAPI]
    public void GetAllChildren(EntityUid root, HashSet<(EntityUid child, EntityUid parent)> outSet, int maxDepth = 256)
    {
        if (maxDepth == 0)
            ThrowRecursionCheck(root);

        foreach (var child in GetChildren(root))
        {
            var res = outSet.Add((child, root));
            if (res)
                ThrowRecursionCheck(root);

            if (ParentQuery.HasComp(child))
                GetAllChildren(child, outSet, maxDepth - 1);
        }
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.GetAllChildren(Robust.Shared.GameObjects.EntityUid,System.Collections.Generic.HashSet{System.ValueTuple{Robust.Shared.GameObjects.EntityUid,Robust.Shared.GameObjects.EntityUid}},System.Int32)"/>
    [PublicAPI]
    [Pure]
    public HashSet<(EntityUid child, EntityUid parent)> GetAllChildren(EntityUid root, int maxDepth = 256)
    {
        var set = new HashSet<(EntityUid child, EntityUid parent)>();

        GetAllChildren(root, set, maxDepth);

        return set;
    }

    /// <summary>
    ///     Enumerates every parent for the given entity.
    /// </summary>
    [PublicAPI]
    [Pure]
    public ParentsEnumerable GetParents(EntityUid root)
    {
        return new ParentsEnumerable(ChildQuery, root);
    }

    // small helper function to avoid having exception machinery in a relatively critical path.
    private static void ThrowRecursionCheck(EntityUid recursingParent)
    {
        throw new RecursionCheckedException(recursingParent);
    }

    /// <summary>
    ///     Determine if two entities hold a direct family relationship in either direction.
    /// </summary>
    /// <returns>Whether either is a parent of the other.</returns>
    [Pure]
    public bool AreDirectlyRelated(EntityUid left, EntityUid right)
    {
        if (TryGetParent(left, out var p1) && p1 == right)
            return true;

        if (TryGetParent(right, out var p2) && p2 == left)
            return true;

        return false;
    }
}

/// <summary>
///     An exception thrown when a <see cref="FamilyEntitySystem{TChild,TParent}"/> runs into potentially recursive operations.
/// </summary>
/// <param name="recursingParent"></param>
public sealed class RecursionCheckedException(EntityUid recursingParent) : Exception
{
    public override string Message =>
        $"Recursion check tripped by ${recursingParent}.";
}

/// <summary>
///     An exception thrown when <see cref="FamilyEntitySystem{TChild,TParent}"/> expects two entities to be related and they are not.
/// </summary>
public sealed class UnrelatedException(EntityUid left, EntityUid right) : Exception
{
    public override string Message => $"{left} and {right} are unrelated when they were expected to be related.";
}
