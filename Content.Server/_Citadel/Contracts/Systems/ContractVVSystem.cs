using System.Linq;
using Content.Server._Citadel.Contracts.Components;

namespace Content.Server._Citadel.Contracts.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class ContractVVSystem : EntitySystem
{
    private static readonly (ViewVariablesPath? Path, string[] Segments) EmptyResolve = (null, Array.Empty<string>());

    [Dependency] private readonly IViewVariablesManager _vv = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        _vv.RegisterDomain("contract", ResolveContract, ListContractPaths);
    }

    private (ViewVariablesPath? Path, string[] Segments) ResolveContract(string path)
    {
        if (string.IsNullOrEmpty(path))
            return EmptyResolve;

        var segments = path.Split('/');

        if (segments.Length == 0)
            return EmptyResolve;

        if (!int.TryParse(segments[0], out var num) || num <= 0)
            return EmptyResolve;

        var uid = new EntityUid(num);

        return (new ViewVariablesInstancePath(uid), segments[1..]);
    }

    private IEnumerable<string>? ListContractPaths(string[] segments)
    {
        if (segments.Length > 1)
            return null;

        if (segments.Length == 1
            && EntityUid.TryParse(segments[0], out var u)
            && Exists(u))
        {
            return null;
        }

        return EntityQuery<ContractComponent>().Select(x => x.Owner.ToString());
    }
}
