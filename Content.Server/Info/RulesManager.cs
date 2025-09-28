using System.Net;
using Content.Server.Administration.Logs;
using Content.Server.Database;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Info;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System.Threading.Tasks;

// Downstream change - needed for Age Gate
using Content.Server._Common.PreJoin;

namespace Content.Server.Info;

// Downstream change - needed for Age Gate: implement IPreJoinAction
public sealed partial class RulesManager : IPreJoinAction
{
    [Dependency] private IServerDbManager _dbManager = default!;
    [Dependency] private INetManager _netManager = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IAdminLogManager _adminLog = default!;
    [Dependency] private IPlayerManager _player = default!;

    // Downstream change - needed for Age Gate
    [Dependency] private PreJoinManager _preJoinManager = default!;

    private static DateTime LastValidReadTime => DateTime.UtcNow - TimeSpan.FromDays(60);

    public void Initialize()
    {
        _netManager.Connected += OnConnected;
        _netManager.RegisterNetMessage<SendRulesInformationMessage>();
        _netManager.RegisterNetMessage<RulesAcceptedMessage>(OnRulesAccepted);

        // Downstream change - needed for Age Gate
        _preJoinManager.RegisterPreJoinAction("Rules", this);
    }

    // Downstream change - needed for Age Gate
    /// <inheritdoc />
    public async Task<bool> PreJoinAction(ICommonSession session)
    {
        var isLocalhost = IPAddress.IsLoopback(session.Channel.RemoteEndPoint.Address) &&
                               _cfg.GetCVar(CCVars.RulesExemptLocal);

        var lastRead = await _dbManager.GetLastReadRules(session.Channel.UserId);
        var hasCooldown = lastRead > LastValidReadTime;

        if (!isLocalhost && !hasCooldown)
        {
            var showRulesMessage = new SendRulesInformationMessage
            {
                PopupTime = _cfg.GetCVar(CCVars.RulesWaitTime),
                CoreRules = _cfg.GetCVar(CCVars.RulesFile),
                ShouldShowRules = true,
            };
            _netManager.ServerSendMessage(showRulesMessage, session.Channel);
            return false;
        }

        return true;
    }

    private async void OnConnected(object? sender, NetChannelArgs e)
    {
        // Downstream change - needed for Age Gate
        var showRulesMessage = new SendRulesInformationMessage
        {
            PopupTime = _cfg.GetCVar(CCVars.RulesWaitTime),
            CoreRules = _cfg.GetCVar(CCVars.RulesFile),
            ShouldShowRules = false,
        };
        _netManager.ServerSendMessage(showRulesMessage, e.Channel);
    }

    private async void OnRulesAccepted(RulesAcceptedMessage message)
    {
        var date = DateTime.UtcNow;
        await _dbManager.SetLastReadRules(message.MsgChannel.UserId, date);
        var session = _player.GetSessionByChannel(message.MsgChannel);

        if (message.FuckRules)
            _adminLog.Add(LogType.Connection, LogImpact.Extreme, $"Player {session} used the fuckrules command.");

        await _preJoinManager.TryJoinGame(session);
    }
}
