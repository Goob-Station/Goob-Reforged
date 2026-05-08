using Robust.Shared.Configuration;

namespace Content.Goobstation.Common.ConVars;

public sealed partial class GoobConVars
{
    public static readonly CVarDef<int> RMCPatronLobbyMessageTimeSeconds =
        CVarDef.Create("rmc.patron_lobby_message_time_seconds", 30, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> RMCPatronLobbyMessageInitialDelaySeconds =
        CVarDef.Create("rmc.patron_lobby_message_initial_delay_seconds", 5, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<string> RMCDiscordAccountLinkingMessageLink =
        CVarDef.Create("rmc.discord_account_linking_message_link", "", CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<string> PatronSupportLastShown =
        CVarDef.Create("patron.support_last_shown", "", CVar.CLIENTONLY | CVar.ARCHIVE);

    public static readonly CVarDef<int> PatronAskSupport =
        CVarDef.Create("patron.ask_support", 7, CVar.REPLICATED | CVar.SERVER);

}
