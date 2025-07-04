// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System.Diagnostics.CodeAnalysis;
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
    ///     Makes the given child and parent related, given the child is not already related to another parent.
    /// </summary>
    /// <param name="child">The child in the relation.</param>
    /// <param name="parent">The parent in the relation.</param>
    public void MakeRelated(Entity<TChild> child, Entity<TParent> parent)
    {
        DebugTools.Assert(child.Comp.Parent == null);

        child.Comp.Parent = parent;
        parent.Comp.Children.Add(child);
    }

    /// <inheritdoc cref="M:Content.Shared._Citadel.Relations.FamilyEntitySystem`2.MakeRelated(Robust.Shared.GameObjects.Entity{`0},Robust.Shared.GameObjects.Entity{`1})"/>
    public void MakeRelated(EntityUid child, EntityUid parent)
    {
        MakeRelated(new Entity<TChild>(child, Comp<TChild>(child)), new Entity<TParent>(parent, Comp<TParent>(parent)));
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
        return TryGetParent(new Entity<TChild>(child, Comp<TChild>(child)), out parent);
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
        return Comp<TChild>(child).Parent!.Value;
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
        return Comp<TParent>(parent).Children;
    }
}

/// <summary>
///     Defines what happens when a relation is dissolved, i.e. an entity is deleted and relations need repaired.
/// </summary>
public enum RelationDissolveBehavior : byte
{
    Clear,
    Reparent,
}
