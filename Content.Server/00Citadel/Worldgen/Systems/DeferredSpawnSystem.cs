using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Server._00OuterRim;

public sealed class DeferredSpawnSystem : EntitySystem
{
    private readonly Stopwatch _simulationStopwatch = new();

    private Queue<(string, EntityCoordinates, DeferredSpawnDoneEvent?)> SpawnQueue = new();

    public override void Update(float frameTime)
    {
        _simulationStopwatch.Restart();
        while (SpawnQueue.Count > 0)
        {
            var dat = SpawnQueue.Dequeue();
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

    public void SpawnEntityDeferred(string prototype, EntityCoordinates position, DeferredSpawnDoneEvent? ev = null)
    {
        SpawnQueue.Enqueue((prototype, position, ev));
    }
}

public abstract class DeferredSpawnDoneEvent : EntityEventArgs
{
    public EntityUid SpawnedEntity;
}
