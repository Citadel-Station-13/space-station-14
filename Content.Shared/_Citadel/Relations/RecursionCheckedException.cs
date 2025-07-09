// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

namespace Content.Shared._Citadel.Relations;

/// <summary>
///     An exception thrown when a <see cref="FamilyEntitySystem{TChild,TParent}"/> runs into potentially recursive operations.
/// </summary>
/// <param name="recursingParent"></param>
public sealed class RecursionCheckedException(EntityUid recursingParent) : Exception
{
    public override string Message =>
        $"Recursion check tripped by ${recursingParent}.";
}
