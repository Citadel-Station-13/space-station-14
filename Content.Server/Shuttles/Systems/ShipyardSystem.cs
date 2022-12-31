using Content.Server.Shuttles.Components;
using Content.Server.Station.Components;
using Content.Server.Cargo.Systems;
using Robust.Server.Maps;
using Robust.Shared.Map;
using Content.Shared.MobState.Components;

namespace Content.Server.Shuttles.Systems;

public sealed partial class ShuttleSystem
{
    public MapId? ShipyardMap { get; private set; }

    [Dependency] private readonly PricingSystem _pricing = default!;
    [Dependency] private readonly CargoSystem _cargo = default!;
    private void InitializeShipyard()
    {
        SubscribeLocalEvent<StationDataComponent, ComponentInit>(OnShipyardStartup);
    }

    private void OnShipyardStartup(EntityUid uid, StationDataComponent component, ComponentInit args)
    {
        SetupShipyard();
    }

    /// <summary>
    /// Adds a ship to the shipyard, calculates its price, and attempts to ftl-dock it to the given station
    /// </summary>
    public void PurchaseShuttle(EntityUid? stationUid, string shuttlePath)
    {
        if (!TryComp<StationDataComponent>(stationUid, out var stationData) || !TryComp<ShuttleComponent>(AddShuttle(shuttlePath), out var shuttle)) return;

        var targetGrid = _station.GetLargestGrid(stationData);

        if (targetGrid == null) return;

        var price = _pricing.AppraiseGrid(shuttle.Owner, null);

        //can do FTLTravel later instead if we want to open that door
        TryFTLDock(shuttle, targetGrid.Value);

        _sawmill.Info($"Shuttle {shuttlePath} was purchased at {targetGrid} for {price} spacebucks");
    }
    /// <summary>
    /// loads a paused shuttle into the ShipyardMap from a file path
    /// </summary>
    private EntityUid? AddShuttle(string shuttlePath)
    {
        if (ShipyardMap == null) return null;

        //only dealing with a single grid at a time for now
        var shuttle = _map.LoadGrid(ShipyardMap.Value, shuttlePath.ToString(), new MapLoadOptions() {Offset = new Vector2(500f + _shuttleIndex, 0f)});

        if (shuttle == null)
        {
            _sawmill.Error($"Unable to spawn shuttle {shuttlePath}");
            return null;
        }

        _shuttleIndex += _mapManager.GetGrid(shuttle.Value).LocalAABB.Width + ShuttleSpawnBuffer;

        return (EntityUid) shuttle;
    }
    public void SellShuttle(EntityUid stationUid, EntityUid shuttleUid)
    {
        if (!TryComp<StationDataComponent>(stationUid, out var stationGrid) || !HasComp<ShuttleComponent>(shuttleUid) || !TryComp<TransformComponent>(shuttleUid, out var xform) || ShipyardMap == null) return;

        var targetGrid = _station.GetLargestGrid(stationGrid);

        if (targetGrid == null) return;

        var gridDocks = GetDocks((EntityUid) targetGrid);
        var shuttleDocks = GetDocks(shuttleUid);
        var isDocked = false;

        foreach (var shuttleDock in shuttleDocks)
        {
            foreach (var gridDock in gridDocks)
            {
                if (shuttleDock.DockedWith == gridDock.Owner)
                {
                    isDocked = true;
                    break;
                };
            };
            if (isDocked) break;
        };

        if (!isDocked)
        {
            _sawmill.Warning($"shuttle is not docked to that station");
            return;
        };

        var mobQuery = GetEntityQuery<MobStateComponent>();
        var xformQuery = GetEntityQuery<TransformComponent>();

        if (_cargo.FoundOrganics(shuttleUid, mobQuery, xformQuery) == true)
        {
            _sawmill.Warning($"organics on board");
            return;
        };

        //just yeet and delete for now. Might want to split it into another function later to send back to the shipyard map first to pause for something
        var price = _pricing.AppraiseGrid(shuttleUid);
        _mapManager.DeleteGrid(shuttleUid);
        _sawmill.Info($"Sold shuttle {shuttleUid} for {price}");
    }
    private void CleanupShipyard()
    {
        if (ShipyardMap == null || !_mapManager.MapExists(ShipyardMap.Value))
        {
            ShipyardMap = null;
            return;
        }

        _mapManager.DeleteMap(ShipyardMap.Value);
    }
    private void SetupShipyard()
    {
        if (ShipyardMap != null && _mapManager.MapExists(ShipyardMap.Value)) return;

        ShipyardMap = _mapManager.CreateMap();

        _mapManager.SetMapPaused(ShipyardMap.Value, true);
    }
}
