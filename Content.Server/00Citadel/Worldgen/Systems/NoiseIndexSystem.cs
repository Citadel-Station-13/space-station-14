using Content.Server._00OuterRim.Worldgen2.Components;
using Content.Server._00OuterRim.Worldgen2.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._00OuterRim.Worldgen2.Systems;

/// <summary>
/// This handles the noise index.
/// </summary>
public sealed class NoiseIndexSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public NoiseGenerator Get(EntityUid holder, string protoId)
    {
        var idx = EnsureComp<NoiseIndexComponent>(holder);
        if (idx.Generators.TryGetValue(protoId, out var generator))
        {
            return generator;
        }
        else
        {
            var proto = _prototype.Index<NoiseChannelPrototype>(protoId);
            var gen = new NoiseGenerator(proto, _random.Next());
            idx.Generators[protoId] = gen;
            return gen;
        }
    }

    public float Evaluate(EntityUid holder, string protoId, Vector2 coords)
    {
        var gen = Get(holder, protoId);
        return gen.Evaluate(coords);
    }
}
