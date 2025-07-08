// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

namespace Content.Shared._Citadel.Relations;

/// <summary>
///     Indicates a component manages a parent-child relationship, as the parent.
/// </summary>
public interface IFamilyRelationParent
{
    /// <summary>
    ///     The children of this entity in the relationship.
    /// </summary>
    public HashSet<EntityUid> Children { get; set;  }
}

