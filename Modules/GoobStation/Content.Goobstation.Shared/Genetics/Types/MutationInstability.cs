// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

namespace Content.Goobstation.Shared.Genetics.Types;

public enum MutationInstability
{
    /// # Negative

    /// <summary>
    /// Negatives that are virtually harmless and mostly just funny (language)
    /// Set to 0 because munchkinning via miscommunication = bad
    /// </summary>
    NegativeNeutral = 0,
    /// <summary>
    /// Ostensibly negatives but not an active hinderance, at least not to you
    /// </summary>
    NegativeMini = -10,
    /// <summary>
    /// Negatives that are slightly annoying (unused)
    /// </summary>
    NegativeMinor = -20,
    /// <summary>
    /// Negatives that present an uncommon or weak, consistent hindrance to gameplay (cough, paranoia)
    /// </summary>
    NegativeModerate = -30,
    /// <summary>
    /// Negatives that present a major consistent hindrance to gameplay (deaf, mute, acid flesh)
    /// </summary>
    NegativeMajor = -40,

    /// # Positive

    /// <summary>
    /// Positives that provide basically no benefit (glowy)
    /// </summary>
    PositiveMini = 5,
    /// <summary>
    /// Positives that are niche in application or useful in rare circumstances (parlor tricks, geladikinesis, autotomy)
    /// </summary>
    PositiveMinor = 10,
    /// <summary>
    /// Positives that provide a new ability that's roughly par with station equipment (insulated, cryokinesis)
    /// </summary>
    PositiveModerate = 25,
    /// <summary>
    /// Positives that are unique, very powerful, and noticeably change combat/gameplay (hulk, tk)
    /// </summary>
    PositiveMajor = 35,
}
