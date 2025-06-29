using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.Map;

namespace Content.Server._Citadel.Overmaps.Systems
{
    public sealed partial class OvermapSystem : SharedOvermapSystem
    {
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

        }

        public EntityUid CreateOvermap(OvermapConfig config)
        {
            return CreateOvermap(config, OvermapGeneration.DoNothing);
        }

        public EntityUid CreateOvermap(OvermapConfig config, OvermapGeneration generation)
        {
            
        }
    }
}
