using Content.Shared._Citadel.Overmaps.Components;
using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps.Systems
{
    public partial class SharedOvermapSystem : EntitySystem
    {
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;

        internal Dictionary<OvermapId, EntityUid> Overmaps { get; } = new();


        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<OvermapComponent, ComponentInit>(OnOvermapInit);
        }

        private void OnOvermapInit(Entity<OvermapComponent> overmap, ref ComponentInit args)
        {
            AssignOvermapId(overmap);
        }

        private void AssignOvermapId(Entity<OvermapComponent> overmap, OvermapId? id = null)
        {
            if (overmap.Comp.OvermapId != null)
            {
                if (id != null && overmap.Comp.OvermapId != id)
                {
                    throw new Exception(
                        $"Overmap entity {ToPrettyString(overmap.Owner)} has already been assigned an overmap id");
                }

                if (Overmaps.TryGetValue((OvermapId)overmap.Comp.OvermapId, out var existing) ||
                    existing != overmap.Owner)
                {
                    throw new Exception(
                        $"Overmap entity {ToPrettyString(overmap.Owner)} was assigned an overmap id that already exists");
                }
            }
        }
    }
}
