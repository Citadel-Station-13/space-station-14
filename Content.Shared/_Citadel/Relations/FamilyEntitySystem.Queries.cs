// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Content.Shared._Citadel.Relations;

public abstract partial class FamilyEntitySystem<TChild, TParent>
{
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
