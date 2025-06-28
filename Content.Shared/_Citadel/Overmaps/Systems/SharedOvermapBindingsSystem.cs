using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps.Systems
{
    public abstract partial class SharedOvermapBindingsSystem : EntitySystem
    {
        protected internal Dictionary<MapId, EntityUid> sectorLookup = new Dictionary<MapId, EntityUid>();

        public override void Initialize()
        {
            base.Initialize();
        }

        public MapId? TryGetRandomBoundMap(EntityUid? uid)
        {

        }

        public MapId? TryGetFirstBoundMap(EntityUid? uid)
        {

        }

        public IEnumerable<MapId>? TryGetBoundMaps (EntityUid? uid)
        {

        }
    }
}
