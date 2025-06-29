using System.Diagnostics.CodeAnalysis;
using Content.Shared._Citadel.Overmaps.Components;

namespace Content.Shared._Citadel.Overmaps.Systems
{
    public partial class SharedOvermapSystem : EntitySystem
    {
        public bool TryGetOvermap([NotNullWhen(true)] OvermapId? overmapId,
            [NotNullWhen(true)] out EntityUid? uid)
        {

        }
    }
}
