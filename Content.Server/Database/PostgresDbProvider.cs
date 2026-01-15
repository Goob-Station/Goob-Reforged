// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using Robust.Shared.Utility;

namespace Content.Server.Database;

public sealed class PostgresDbProvider : IDbProvider
{
    public bool SupportsIpRangeQueries => true;

    public DateTime NormalizeDatabaseTime(DateTime time)
    {
        DebugTools.Assert(time.Kind == DateTimeKind.Utc);
        return time;
    }
}
