using Content.Shared.Administration;
using Content.Shared.CCVar.CVarAccess;
using Robust.Shared.Configuration;

namespace Content.Shared._Citadel.CCVar;

public sealed partial class CitCVars
{

    /// <summary>
    ///     Whether or not character generation allows unlimited points and arbitrary
    ///     layer ordering of markings
    /// </summary>
    [CVarControl(AdminFlags.Server)]
    public static readonly CVarDef<bool> ChargenAnarchicLayering =
        CVarDef.Create("citadel.chargen.anarchic_layering", true, CVar.SERVER | CVar.REPLICATED);
}