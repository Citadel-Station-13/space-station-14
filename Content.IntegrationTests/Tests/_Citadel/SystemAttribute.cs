// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

namespace Content.IntegrationTests.Tests._Citadel;

/// <summary>
///     Marks a field on a CitadelGameTest inheritor as needing to be populated with a system from the given side.
/// </summary>
public sealed class SystemAttribute : Attribute
{
    public SystemAttribute(Side side)
    {
        Side = side;

        if (side == Side.Neither)
        {
            throw new NotSupportedException();
        }
    }

    public Side Side { get; }
}

/// <summary>
///     Marks a field on a CitadelGameTest inheritor as needing to be populated with an IoC dependency from the given side.
/// </summary>
public sealed class SidedDependencyAttribute : Attribute
{
    public SidedDependencyAttribute(Side side)
    {
        Side = side;

        if (side == Side.Neither)
        {
            throw new NotSupportedException();
        }
    }
    public Side Side { get; }
}

public enum Side
{
    Client,
    Server,
    // A special value meant as a default for attributes, and NOTHING ELSE.
    Neither
}
