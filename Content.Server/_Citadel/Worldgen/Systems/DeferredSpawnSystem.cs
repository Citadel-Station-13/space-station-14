using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Server._Citadel.Worldgen.Systems;

/// <summary>
/// Used to delay spawning entities under load to avoid overrunning the tick.
/// </summary>
public sealed class DeferredSpawnSystem : EntitySystem
{
    private readonly Stopwatch _simulationStopwatch = new();

    private Queue<(string, EntityCoordinates, DeferredSpawnDoneEvent?)> _spawnQueue = new();

    /// <inheritdoc/>
    public override void Update(float frameTime)
    {
        _simulationStopwatch.Restart();
        while (_spawnQueue.Count > 0)
        {
            var dat = _spawnQueue.Dequeue();
            var e = Spawn(dat.Item1, dat.Item2);

            if (dat.Item3 is not null)
            {
                dat.Item3.SpawnedEntity = e;
                QueueLocalEvent(dat.Item3);
            }

            if (_simulationStopwatch.Elapsed.Milliseconds > 5)
                break;
        }
    }

    /// <summary>
    /// Registers a entity to be spawned later when there's available time.
    /// </summary>
    /// <param name="prototype">Prototype to spawn.</param>
    /// <param name="position">The position to spawn at.</param>
    /// <param name="ev">The event to fire when spawning is complete.</param>
    public void SpawnEntityDeferred(string prototype, EntityCoordinates position, DeferredSpawnDoneEvent? ev = null)
    {
        _spawnQueue.Enqueue((prototype, position, ev));
    }
}

public abstract class DeferredSpawnDoneEvent : EntityEventArgs
{
    public EntityUid SpawnedEntity;
}
