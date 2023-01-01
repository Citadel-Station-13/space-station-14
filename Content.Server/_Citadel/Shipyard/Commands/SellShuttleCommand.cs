using Content.Server.Administration;
using Content.Server.Shipyard.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Shipyard.Commands;

/// <summary>
/// sells a shuttle.
/// </summary>
[AdminCommand(AdminFlags.Fun)]
public sealed class SellShuttleCommand : IConsoleCommand
{
    public string Command => "sellshuttle";
    public string Description => "appraises and sells a selected grid connected to selected station";
    public string Help => $"{Command}";
    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (!EntityUid.TryParse(args[0], out var stationId))
        {
            shell.WriteError($"{args[0]} is not a valid entity uid.");
            return;
        }
        if (!EntityUid.TryParse(args[1], out var shuttleId))
        {
            shell.WriteError($"{args[0]} is not a valid entity uid.");
            return;
        };
        var system = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ShipyardSystem>();
        system.SellShuttle(stationId, shuttleId);
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        switch (args.Length)
        {
            case 1:
                return CompletionResult.FromHint(Loc.GetString("station-id"));
            case 2:
                return CompletionResult.FromHint(Loc.GetString("shuttle-id"));
        }
        return CompletionResult.Empty;
    }
}
