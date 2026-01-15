// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Content.Server.Database;

/// <summary>
/// Abstracts differences between database providers (PostgreSQL, SQLite, etc.)
/// so ServerDbBase can have shared implementations without provider-specific code.
///
/// This is boilerplate.
/// </summary>
public interface IDbProvider
{
    DateTime NormalizeDatabaseTime(DateTime time);
    bool SupportsIpRangeQueries { get; }
}
