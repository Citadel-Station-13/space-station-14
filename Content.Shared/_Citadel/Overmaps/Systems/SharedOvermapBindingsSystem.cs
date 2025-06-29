using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._Citadel.Overmaps.Components;
using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps.Systems
{
    /// <summary>
    /// Shared overmap bindings entity system.
    /// Not many functions live here, as overmap bindings are handled serverside.
    /// </summary>
    public abstract partial class SharedOvermapBindingsSystem : EntitySystem;
}
