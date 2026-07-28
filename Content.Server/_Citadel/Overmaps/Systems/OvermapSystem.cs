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

        /// <summary>
        /// Creates an overmap, without performing generation on it.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public EntityUid CreateOvermap(OvermapConfig config)
        {
            return CreateOvermap(config, OvermapGeneration.DoNothing);
        }

        public EntityUid CreateOvermap(OvermapConfig config, OvermapGeneration generation)
        {

        }
    }
}
