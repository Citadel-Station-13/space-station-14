// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

namespace Content.Shared._Citadel.Relations.Testing;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class TestFamilyRelationComponent : Component, IFamilyRelationParent, IFamilyRelationChild
{
    [DataField]
    public HashSet<EntityUid> Children { get; set; } = new();
    [DataField]
    public EntityUid? Parent { get; set; } = null;
}
