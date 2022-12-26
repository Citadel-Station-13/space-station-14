using Content.Server.Shuttles.Components;
using Content.Server.Station.Components;
using Content.Server.Cargo.Systems;
using Robust.Server.Maps;
using Robust.Shared.Map;

namespace Content.Server.Shuttles.Systems;

public sealed partial class ShuttleSystem
{
    public MapId? ShipyardMap { get; private set; }

    [Dependency] private readonly PricingSystem _pricing = default!;
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

        TryFTLDock(shuttle, targetGrid.Value);

        _sawmill.Warning($"Shuttle {shuttlePath} was purchased at {targetGrid} for {price} spacebucks");
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
