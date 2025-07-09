// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

namespace Content.Shared._Citadel.Relations;

/// <summary>
///     An exception thrown when <see cref="FamilyEntitySystem{TChild,TParent}"/> expects two entities to be related and they are not.
/// </summary>
public sealed class UnrelatedException(EntityUid left, EntityUid right) : Exception
{
    public override string Message => $"{left} and {right} are unrelated when they were expected to be related.";
}
