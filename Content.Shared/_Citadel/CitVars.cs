using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Citadel.CitVars
{
    [CVarDefs]
    public sealed class CitVars : CVars
    {
        /// <summary>
        ///     Automatically spawns more contracts as contracts are cleared
        /// </summary>
        public static readonly CVarDef<bool>
            CitDebugContractSpawning = CVarDef.Create("debug.citadel.contractspawning", false, CVar.ARCHIVE | CVar.SERVERONLY);
    }
}