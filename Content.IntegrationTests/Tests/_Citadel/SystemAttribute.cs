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
public sealed class SystemAttribute(Side side) : Attribute
{
    public Side Side { get; } = side;
}

/// <summary>
///     Marks a field on a CitadelGameTest inheritor as needing to be populated with an IoC dependency from the given side.
/// </summary>
public sealed class SidedDependencyAttribute(Side side) : Attribute
{
    public Side Side { get; } = side;
}

public enum Side
{
    Client,
    Server
}
