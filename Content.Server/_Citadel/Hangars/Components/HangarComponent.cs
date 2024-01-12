/// WIP! WIP! WIP!
// ALL RIGHTS RESERVED TO CITADEL-STATION-13 ORG MEMBERS UNTIL FURTHER NOTICE
// PLEASE DO NOT USE THIS CODE IN YOUR OWN PROJECTS UNTIL A PROPER MPL VARIANT IS WRITTEN
// BESIDES, THIS IS SO HEAVILY WIP THAT WE'D BE GENUINELY SURPRISED IF THIS IS USABLE AT ALL
/// WIP! WIP! WIP!

using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Hangars.Components;

/// <summary>
/// Component that creates a hangar in it's own isolated map. Primarily useful for contracts
/// </summary>
[RegisterComponent]
public sealed partial class HangarComponent : Component
{
    /// <summary>
    /// Map to use for this hangar
    /// </summary>
    [DataField("hangarPath", customTypeSerializer: typeof(ResPathSerializer))]
    public ResPath HangarPath { get; set; } = new("/Maps/Test/admin_test_arena.yml");

    public EntityUid? HangarMapUid;
    public MapId? HangarMapID;
}
