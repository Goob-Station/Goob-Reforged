// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using System.Text;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem
{
    /// <summary>Handles announcements.</summary>
    /// <param name="ent">Entity to parse announcements for</param>
    private void HandleAnnouncements(Entity<SupermatterComponent> ent)
    {
        var sm = ent.Comp;

        // Is a delamination actively starting?
        if (sm is { Delamming: true, DelamAnnounced: false })
        {
            AnnounceDelamStart(ent);
            return;
        }

        // Was a delamination just averted?
        if (sm is { Delamming: true } && sm.Damage < sm.DelaminationPoint)
        {
            AnnounceDelamStop(ent);
            return;
        }

        // If we are not actively taking damage, skip routine warnings.
        if (sm.DamageDelta <= 0)
        {
            return;
        }

        // Handle routine damage thresholds.
        HandleDamageWarnings(ent);
    }

    private void AnnounceDelamStart(Entity<SupermatterComponent> ent)
    {
        var sm = ent.Comp;
        var (locId, alertLevel) = GetDelamAlertDetails(ent);

        // Alert the station
        if (_station.GetOwningStation(ent) is { } station)
        {
            _alert.SetLevel(station, alertLevel, playSound: true, announce: true, force: true);
        }

        // Build and dispatch the announcement
        var sb = new StringBuilder();
        sb.AppendLine(Loc.GetString(locId))
            .AppendLine(Loc.GetString("supermatter-seconds-before-delam", ("seconds", MathF.Floor(sm.DelamDuration))));

        sm.DelamAnnounced = true;
        _chat.DispatchSupermatterAnnouncement(ent, sb.ToString(), global: true);
    }

    private void AnnounceDelamStop(Entity<SupermatterComponent> ent)
    {
        var sm = ent.Comp;
        var integrity = sm.IntegrityString;
        var message = Loc.GetString("supermatter-delam-cancel", ("integrity", integrity));

        sm.DelamAnnounced = false;
        _chat.DispatchSupermatterAnnouncement(ent, message, global: true);
    }

    private void HandleDamageWarnings(Entity<SupermatterComponent> ent)
    {
        var sm = ent.Comp;
        var integrity = sm.IntegrityString;

        string message;
        var global = false;

        switch (sm.Damage)
        {
            case >= SupermatterComponent.EmergencyPoint:
                message = Loc.GetString("supermatter-emergency", ("integrity", integrity));
                global = true;
                break;
            case >= SupermatterComponent.WarningPoint:
                message = Loc.GetString("supermatter-warning", ("integrity", integrity));
                break;
            default:
                return; // No warning threshold met
        }

        _chat.DispatchSupermatterAnnouncement(ent, message, global);
    }
    private (string LocId, string AlertLevel) GetDelamAlertDetails(Entity<SupermatterComponent> ent)
    {
        return GetDelamType(ent) switch
        {
            DelamType.Singulo => ("supermatter-delam-overmass", "delta"),
            DelamType.Tesla => ("supermatter-delam-tesla", "delta"),
            DelamType.Cascade => ("supermatter-delam-cascade", "delta"),
            _ => ("supermatter-delam-explosion", "yellow")
        };
    }
}
