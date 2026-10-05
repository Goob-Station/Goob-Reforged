// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Systems;
using Robust.Shared.Audio;

namespace Content.Goobstation.Shared.Genetics.Components;

[RegisterComponent]
[Access(typeof(HandheldGeneticsScannerSystem))]
public sealed partial class HandheldGeneticsScannerComponent : Component
{
    /// <summary>
    /// How long it takes to scan someone.
    /// </summary>
    [DataField]
    public TimeSpan ScanDelay = TimeSpan.FromSeconds(0.8);

    /// <summary>
    /// The maximum range in tiles at which the analyzer can scan.
    /// </summary>
    [DataField]
    public float? MaxScanRange = 2.5f;

    /// <summary>
    /// Sound played on scanning begin.
    /// </summary>
    [DataField]
    public SoundSpecifier? ScanningBeginSound;

    /// <summary>
    /// Sound played on scanning end.
    /// </summary>
    [DataField]
    public SoundSpecifier ScanningEndSound = new SoundPathSpecifier("/Audio/Items/Medical/healthscanner.ogg");

    [DataField]
    public LocId LocReportTitle = "genetics-handheld-scanner-report-title";

    [DataField]
    public LocId LocNotMutable = "genetics-handheld-scanner-error-not-mutable";

    [DataField]
    public LocId LocNotOrganic = "genetics-handheld-scanner-error-inorganic";
};
