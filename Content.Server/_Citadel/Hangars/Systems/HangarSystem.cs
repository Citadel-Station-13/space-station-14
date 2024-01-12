/// WIP! WIP! WIP!
// ALL RIGHTS RESERVED TO CITADEL-STATION-13 ORG MEMBERS UNTIL FURTHER NOTICE
// PLEASE DO NOT USE THIS CODE IN YOUR OWN PROJECTS UNTIL A PROPER MPL VARIANT IS WRITTEN
// BESIDES, THIS IS SO HEAVILY WIP THAT WE'D BE GENUINELY SURPRISED IF THIS IS USABLE AT ALL
/// WIP! WIP! WIP!

using Content.Server._Citadel.Hangars.Components;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Server.GameObjects;

namespace Content.Server._Citadel.Hangars.Systems;

public sealed class HangarSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IMapManager _mapMan = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly MetaDataSystem _metaDataSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<HangarComponent, ComponentInit>(OnHangarInit);
        SubscribeLocalEvent<HangarComponent, ComponentRemove>(OnHangarRemove);
    }

    private void OnHangarInit(EntityUid uid, HangarComponent comp, ComponentInit args)
    {
        var mapID = _mapMan.CreateMap();
        var mapUid = _mapMan.GetMapEntityId(mapID);
        comp.HangarMapID = mapID;
        comp.HangarMapUid = mapUid;
        _mapMan.AddUninitializedMap(mapID);
        var grid = _entMan.EnsureComponent<MapGridComponent>(mapUid);
        _mapLoader.Load(mapID, comp.HangarPath.ToString());
        _mapMan.DoMapInitialize(mapID);

        var ftlUid = _entMan.CreateEntityUninitialized("FTLPoint", new EntityCoordinates(mapUid, grid.TileSizeHalfVector));
        _metaDataSystem.SetEntityName(ftlUid, $"Hangar #{(int) uid}");
        _entMan.InitializeAndStartEntity(ftlUid);
    }

    private void OnHangarRemove(EntityUid uid, HangarComponent comp, ComponentRemove args)
    {
        QueueDel(comp.HangarMapUid);
    }
}