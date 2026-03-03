// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0


using Content.Goobstation.Shared.Supermatter.Components;
using System.Text;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem
{
    /// <summary>
    ///     Handles announcements.
    /// </summary>
    private void HandleAnnouncements(Entity<SupermatterComponent> ent)
    {
        var message = string.Empty;
        var global = false;

        var integrity = ent.Comp.GetIntegrity().ToString("0.00");

        // Delam is happening
        if (ent.Comp is { Delamming: true, DelamAnnounced: false })
        {
            var sb = new StringBuilder();
            var alertLevel = "yellow";

            string? loc;
            switch (GetDelamType(ent))
            {
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

            var station = _station.GetOwningStation(ent);
            if (station != null)
                _alert.SetLevel((EntityUid)station, alertLevel, true, true, true);

            sb.AppendLine(Loc.GetString(loc));
            sb.AppendLine(Loc.GetString("supermatter-seconds-before-delam", ("seconds", ent.Comp.DelamTimer)));

            message = sb.ToString();
            global = true;
            ent.Comp.DelamAnnounced = true;

            _chat.DispatchSupermatterAnnouncement(ent, message, global);
            return;
        }

        // Delam stopped, let everyone know.
        if (ent.Comp.Damage < ent.Comp.DelaminationPoint && ent.Comp.Delamming)
        {
            message = Loc.GetString("supermatter-delam-cancel", ("integrity", integrity));
            ent.Comp.DelamAnnounced = false;
            global = true;
            _chat.DispatchSupermatterAnnouncement(ent, message, global);
            return;
        }

        // We are not taking consistent damage. Engis/warn not needed.
        if (ent.Comp.DamageDelta >= 0)
            return;

        // Check if we need to warn anyone
        switch (ent.Comp.Damage)
        {
            case >= SupermatterComponent.EmergencyPoint:
                message = Loc.GetString("supermatter-emergency", ("integrity", integrity));
                global = true;
                break;
            case >= SupermatterComponent.WarningPoint:
                message = Loc.GetString("supermatter-warning", ("integrity", integrity));
                break;
        }

        _chat.DispatchSupermatterAnnouncement(ent, message, global);
    }
}
