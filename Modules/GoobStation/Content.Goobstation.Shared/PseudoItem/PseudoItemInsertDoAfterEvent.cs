// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: LicenseRef-GAG-1.0.txt

using Robust.Shared.Serialization;
using Content.Shared.DoAfter;

namespace Content.Goobstation.Shared.PseudoItem;


[Serializable, NetSerializable]
public sealed partial class PseudoItemInsertDoAfterEvent : SimpleDoAfterEvent
{
}
