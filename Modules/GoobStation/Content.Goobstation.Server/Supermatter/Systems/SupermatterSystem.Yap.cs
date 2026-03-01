// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0


using Content.Goobstation.Shared.Supermatter.Components;
using Content.Goobstation.Shared.Supermatter.Systems;
using Content.Server.AlertLevel;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Shared.Chat;
using Robust.Shared.Maths;
using System.Text;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem : SharedSupermatterSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly AlertLevelSystem _alert = default!;
    [Dependency] private readonly StationSystem _station = default!;

    /// <summary>
    ///     Handles announcements.
    /// </summary>
    private void HandleAnnouncements(EntityUid uid, SupermatterComponent sm)
    {
        var message = string.Empty;
        var global = false;

        var integrity = sm.GetIntegrity().ToString("0.00");

        // Delam is happening
        if (sm.Delamming && !sm.DelamAnnounced)
        {
            var sb = new StringBuilder();
            var alertLevel = "yellow";

            string? loc;
            switch (sm.DelamType)
            {
                case DelamType.Explosion:
                default:
                    loc = "supermatter-delam-explosion";
                    break;

                case DelamType.Singulo:
                    loc = "supermatter-delam-overmass";
                    alertLevel = "delta";
                    break;

                case DelamType.Tesla:
                    loc = "supermatter-delam-tesla";
                    alertLevel = "delta";
                    break;

                case DelamType.Cascade:
                    loc = "supermatter-delam-cascade";
                    alertLevel = "delta";
                    break;
            }

            var station = _station.GetOwningStation(uid);
            if (station != null)
                _alert.SetLevel((EntityUid)station, alertLevel, true, true, true, false);

            sb.AppendLine(Loc.GetString(loc));
            sb.AppendLine(Loc.GetString("supermatter-seconds-before-delam", ("seconds", sm.DelamTimer)));

            message = sb.ToString();
            global = true;
            sm.DelamAnnounced = true;

            _chat.SupermatterAnnouncement(uid, message, global);
            return;
        }

        // Delam stopped, let everyone know.
        if (sm.Damage < sm.DelaminationPoint && sm.Delamming)
        {
            message = Loc.GetString("supermatter-delam-cancel", ("integrity", integrity));
            sm.DelamAnnounced = false;
            global = true;
            _chat.SupermatterAnnouncement(uid, message, global);
            return;
        }

        // We are not taking consistent damage. Engis/warn not needed.
        if (sm.DamageDelta >= 0)
            return;

        // Check if we need to warn anyone
        switch (sm.Damage)
        {
            case >= SupermatterComponent.EmergencyPoint:
                message = Loc.GetString("supermatter-emergency", ("integrity", integrity));
                global = true;
                break;
            case >= SupermatterComponent.WarningPoint:
                message = Loc.GetString("supermatter-warning", ("integrity", integrity));
                break;
        }

        _chat.SupermatterAnnouncement(uid, message, global);
    }
}

internal static partial class SupermatterExtensions
{
    /// <summary>
    ///     Help the SM announce something.
    /// </summary>
    /// <param name="global">If true, does the station announcement.</param>
    /// <param name="customSender">If true, sends the announcement from Central Command.</param>
    public static void SupermatterAnnouncement(this ChatSystem chat, EntityUid uid, string message, bool global = false, string? customSender = null)
    {
        if (global)
        {
            var sender = customSender ?? Loc.GetString("supermatter-announcer");
            chat.DispatchStationAnnouncement(uid, message, sender, colorOverride: Color.Yellow);
            return;
        }
        chat.TrySendInGameICMessage(uid, message, InGameICChatType.Speak, hideChat: false, checkRadioPrefix: true);
    }

}
