using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._Citadel.Overmaps.Components;
using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps.Systems
{
    public abstract partial class SharedOvermapBindingsSystem : EntitySystem
    {
        protected internal Dictionary<MapId, EntityUid> sectorLookup = new Dictionary<MapId, EntityUid>();
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;

        private EntityQuery<OvermapBindingsComponent> _overmapBindingsQuery;

        public override void Initialize()
        {
            base.Initialize();

            _overmapBindingsQuery = GetEntityQuery<OvermapBindingsComponent>();

            SubscribeNetworkEvent();
        }


        public MapId? TryGetRandomBoundMapId(EntityUid? uid)
        {
            if (_overmapBindingsQuery.TryGetComponent(uid, out var overmapBindings))
            {
                return overmapBindings.MapId;
            }

            return null;
        }

        public MapId? TryGetFirstBoundMapId(EntityUid? uid)
        {
            if (_overmapBindingsQuery.TryGetComponent(uid, out var overmapBindings))
            {
                return overmapBindings.MapId;
            }

            return null;
        }

        /// <summary>
        /// Returns bound maps as enumerable.
        /// </summary>
        /// <param name="uid">Entity UID of the overmapped entity with bindings</param>
        public IEnumerable<MapId>? TryGetBoundMapIds (EntityUid? uid)
        {
            if (!_overmapBindingsQuery.TryGetComponent(uid, out var overmapBindings))
                return null;
            // todo this is shit
            var l = new List<MapId> { overmapBindings.MapId };
            return l.AsEnumerable();
        }

        /// <summary>
        /// Returns bound maps in ascending order as enumerable.
        /// </summary>
        /// <param name="uid">Entity UID of the overmapped entity with bindings</param>
        public IEnumerable<MapId>? TryGetZAscendingBoundMapIds (EntityUid? uid)
        {
            if (!_overmapBindingsQuery.TryGetComponent(uid, out var overmapBindings))
                return null;
            // todo this is shit
            var l = new List<MapId> { overmapBindings.MapId };
            return l.AsEnumerable();
        }
    }
}
