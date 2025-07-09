// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

namespace Content.Shared._Citadel.Relations;

public sealed class MixedParentChildDisallowedException(Type child, Type parent) : Exception
{
    public override string Message =>
        $"Mixing Parent and Child components on the same entity is disallowed for {child} and {parent}";
}
