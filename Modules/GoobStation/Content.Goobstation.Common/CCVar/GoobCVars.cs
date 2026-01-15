// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Robust.Shared.Configuration;

namespace Content.Goobstation.Common.CCVar;

/// <summary>
/// Goobstation-specific CVars.
/// </summary>
[CVarDefs]
public sealed class GoobCVars
{
    #region Database

    public static readonly CVarDef<string> GoobDatabaseEngine =
        CVarDef.Create("goob.database.engine", "sqlite", CVar.SERVERONLY);

    public static readonly CVarDef<string> GoobDatabaseSqlitePath =
        CVarDef.Create("goob.database.sqlite_path", "goobstation.db", CVar.SERVERONLY);

    public static readonly CVarDef<string> GoobDatabasePgHost =
        CVarDef.Create("goob.database.pg_host", "localhost", CVar.SERVERONLY);

    public static readonly CVarDef<int> GoobDatabasePgPort =
        CVarDef.Create("goob.database.pg_port", 5432, CVar.SERVERONLY);

    public static readonly CVarDef<string> GoobDatabasePgDatabase =
        CVarDef.Create("goob.database.pg_database", "goobstation", CVar.SERVERONLY);

    public static readonly CVarDef<string> GoobDatabasePgUsername =
        CVarDef.Create("goob.database.pg_username", "postgres", CVar.SERVERONLY);

    public static readonly CVarDef<string> GoobDatabasePgPassword =
        CVarDef.Create("goob.database.pg_password", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    #endregion
}
