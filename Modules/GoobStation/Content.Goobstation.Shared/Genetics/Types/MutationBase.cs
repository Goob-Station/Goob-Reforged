// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

namespace Content.Goobstation.Shared.Genetics.Types;

public static partial class MutationBase
{
    public static readonly char[] Bases = ['A', 'T', 'G', 'C'];

    public static char GetMatchingBase(char b)
    => b switch
    {
        'A' => 'T',
        'T' => 'A',
        'G' => 'C',
        'C' => 'G',
        _ => 'X'
    };
}
