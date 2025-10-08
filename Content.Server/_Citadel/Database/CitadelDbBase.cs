// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

namespace Content.Server._Citadel.Database;

public abstract class CitadelDbBase(ILogManager logManager)
{
    private readonly ISawmill _sawmill = logManager.GetSawmill("citadel.db");
}
