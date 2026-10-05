// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

namespace Content.Goobstation.Shared.Genetics.Types;

public record struct MutationStatus(
    bool HasMutation,
    bool HasMutationDormant
);
