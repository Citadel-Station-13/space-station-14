// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

namespace Content.Shared._Citadel.Relations;

public abstract partial class FamilyEntitySystem<TChild, TParent>
{
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
}
