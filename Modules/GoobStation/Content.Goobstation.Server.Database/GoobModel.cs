// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Content.Goobstation.Server.Database;

[Table("netspeak_words")]
public class NetspeakWord
{
    [Key]
    public int Id { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
