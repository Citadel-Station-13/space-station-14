using Robust.Shared.Map;

namespace Content.Server._Citadel.Overmaps
{
    public partial class SharedOvermapSystem : EntitySystem
    {
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
