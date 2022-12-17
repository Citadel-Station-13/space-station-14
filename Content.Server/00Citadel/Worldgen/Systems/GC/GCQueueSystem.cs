using System.Linq;
using Content.Server._00OuterRim.Worldgen2.Components.GC;
using Content.Server._00OuterRim.Worldgen2.Prototypes;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._00OuterRim.Worldgen2.Systems.GC;

/// <summary>
/// This handles...
/// </summary>
public sealed class GCQueueSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private Dictionary<string, Queue<EntityUid>> _queues = new();
    private TimeSpan _maximumProcessTime = TimeSpan.Zero;

    /// <inheritdoc/>
    public override void Initialize()
    {
        _cfg.OnValueChanged(WorldgenCVars.GCMaximumTimeMs, s => _maximumProcessTime = TimeSpan.FromMilliseconds(s), true);
    }

    public override void Update(float frameTime)
    {
        var overallWatch = new Stopwatch();
        var queueWatch = new Stopwatch();
        var queues = _queues.ToList();
        _random.Shuffle(queues); // Avert resource starvation by always processing in random order.
        overallWatch.Start();
        foreach (var (pId, queue) in queues)
        {
            if (overallWatch.Elapsed < _maximumProcessTime)
                return;

            var proto = _proto.Index<GCQueuePrototype>(pId);
            if (queue.Count <= proto.MinDepthToProcess)
                continue;

            queueWatch.Restart();
            while (queueWatch.Elapsed < proto.MaximumTickTime && queue.Count > proto.MinDepthToProcess && queue.Count != 0 && overallWatch.Elapsed < _maximumProcessTime)
            {
                var e = queue.Dequeue();
                if (!Deleted(e))
                    Del(e);
            }
        }
    }

    /// <summary>
    /// Attempts to GC an entity. This functions as QueueDel if it can't.
    /// </summary>
    /// <param name="e">Entity to GC.</param>
    public void TryGCEntity(EntityUid e)
    {
        if (!TryComp<GCAbleObjectComponent>(e, out var comp))
        {
            QueueDel(e); // not our problem :)
            return;
        }

        var queue = _queues[comp.Queue];
        var proto = _proto.Index<GCQueuePrototype>(comp.Queue);
        if (queue.Count > proto.Depth)
        {
            QueueDel(e); // whelp, too full.
            return;
        }

        if (proto.TrySkipQueue)
        {
            var ev = new TryGCImmediately();
            RaiseLocalEvent(e, ref ev);
            if (!ev.Cancelled)
            {
                QueueDel(e);
                return;
            }
        }

        queue.Enqueue(e);
    }
}


[ByRefEvent]
public record struct TryGCImmediately(bool Cancelled = false);
