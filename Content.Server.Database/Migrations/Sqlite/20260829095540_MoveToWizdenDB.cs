using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class MoveToWizdenDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "admin",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "admin_rank_id",
                table: "admin",
                newName: "AdminRankId");

            migrationBuilder.RenameTable(
                name: "admin_flag",
                newName: "AdminFlag");

            migrationBuilder.RenameColumn(
                name: "admin_flag_id",
                table: "AdminFlag",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "admin_id",
                table: "AdminFlag",
                newName: "AdminId");

            migrationBuilder.RenameTable(
                name: "admin_log",
                newName: "AdminLog");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminLog",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "admin_log_id",
                table: "AdminLog",
                newName: "Id");

            migrationBuilder.RenameTable(
                name: "admin_log_player",
                newName: "AdminLogPlayer");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminLogPlayer",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "log_id",
                table: "AdminLogPlayer",
                newName: "LogId");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "AdminLogPlayer",
                newName: "PlayerUserId");

            migrationBuilder.RenameTable(
                name: "admin_messages",
                newName: "AdminMessages");

            migrationBuilder.RenameColumn(
                name: "admin_messages_id",
                table: "AdminMessages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AdminMessages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "AdminMessages",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "AdminMessages",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "deleted_by_id",
                table: "AdminMessages",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "expiration_time",
                table: "AdminMessages",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "last_edited_at",
                table: "AdminMessages",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "last_edited_by_id",
                table: "AdminMessages",
                newName: "LastEditedById");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "AdminMessages",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "playtime_at_note",
                table: "AdminMessages",
                newName: "PlaytimeAtNote");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminMessages",
                newName: "RoundId");

            migrationBuilder.RenameTable(
                name: "admin_notes",
                newName: "AdminNotes");

            migrationBuilder.RenameColumn(
                name: "admin_notes_id",
                table: "AdminNotes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AdminNotes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "AdminNotes",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "AdminNotes",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "deleted_by_id",
                table: "AdminNotes",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "expiration_time",
                table: "AdminNotes",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "last_edited_at",
                table: "AdminNotes",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "last_edited_by_id",
                table: "AdminNotes",
                newName: "LastEditedById");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "AdminNotes",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "playtime_at_note",
                table: "AdminNotes",
                newName: "PlaytimeAtNote");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminNotes",
                newName: "RoundId");

            migrationBuilder.RenameTable(
                name: "admin_rank",
                newName: "AdminRank");

            migrationBuilder.RenameColumn(
                name: "admin_rank_id",
                table: "AdminRank",
                newName: "Id");

            migrationBuilder.RenameTable(
                name: "admin_rank_flag",
                newName: "AdminRankFlag");

            migrationBuilder.RenameColumn(
                name: "admin_rank_flag_id",
                table: "AdminRankFlag",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "admin_rank_id",
                table: "AdminRankFlag",
                newName: "AdminRankId");

            migrationBuilder.RenameTable(
                name: "admin_watchlists",
                newName: "AdminWatchlists");

            migrationBuilder.RenameColumn(
                name: "admin_watchlists_id",
                table: "AdminWatchlists",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AdminWatchlists",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "AdminWatchlists",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "AdminWatchlists",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "deleted_by_id",
                table: "AdminWatchlists",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "expiration_time",
                table: "AdminWatchlists",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "last_edited_at",
                table: "AdminWatchlists",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "last_edited_by_id",
                table: "AdminWatchlists",
                newName: "LastEditedById");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "AdminWatchlists",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "playtime_at_note",
                table: "AdminWatchlists",
                newName: "PlaytimeAtNote");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminWatchlists",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "antag_id",
                table: "antag",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "antag_name",
                table: "antag",
                newName: "AntagName");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "antag",
                newName: "ProfileId");

            migrationBuilder.RenameTable(
                name: "assigned_user_id",
                newName: "AssignedUserId");

            migrationBuilder.RenameColumn(
                name: "assigned_user_id_id",
                table: "AssignedUserId",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AssignedUserId",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "AssignedUserId",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "ban",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "auto_delete",
                table: "ban",
                newName: "AutoDelete");

            migrationBuilder.RenameColumn(
                name: "ban_time",
                table: "ban",
                newName: "BanTime");

            migrationBuilder.RenameColumn(
                name: "banning_admin",
                table: "ban",
                newName: "BanningAdmin");

            migrationBuilder.RenameColumn(
                name: "exempt_flags",
                table: "ban",
                newName: "ExemptFlags");

            migrationBuilder.RenameColumn(
                name: "expiration_time",
                table: "ban",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "last_edited_at",
                table: "ban",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "last_edited_by_id",
                table: "ban",
                newName: "LastEditedById");

            migrationBuilder.RenameColumn(
                name: "playtime_at_note",
                table: "ban",
                newName: "PlaytimeAtNote");

            migrationBuilder.RenameTable(
                name: "ban_address",
                newName: "BanAddress");

            migrationBuilder.RenameColumn(
                name: "ban_address_id",
                table: "BanAddress",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanAddress",
                newName: "BanId");

            migrationBuilder.RenameTable(
                name: "ban_hwid",
                newName: "BanHwid");

            migrationBuilder.RenameColumn(
                name: "ban_hwid_id",
                table: "BanHwid",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanHwid",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "hwid_type",
                table: "BanHwid",
                newName: "HWId_Type");

            migrationBuilder.RenameTable(
                name: "ban_player",
                newName: "BanPlayer");

            migrationBuilder.RenameColumn(
                name: "ban_player_id",
                table: "BanPlayer",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanPlayer",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "BanPlayer",
                newName: "UserId");

            migrationBuilder.RenameTable(
                name: "ban_role",
                newName: "BanRole");

            migrationBuilder.RenameColumn(
                name: "ban_role_id",
                table: "BanRole",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanRole",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "BanRole",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "role_type",
                table: "BanRole",
                newName: "RoleType");

            migrationBuilder.RenameTable(
                name: "ban_round",
                newName: "BanRound");

            migrationBuilder.RenameColumn(
                name: "ban_round_id",
                table: "BanRound",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanRound",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "BanRound",
                newName: "RoundId");

            migrationBuilder.RenameTable(
                name: "ban_template",
                newName: "BanTemplate");

            migrationBuilder.RenameColumn(
                name: "ban_template_id",
                table: "BanTemplate",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "auto_delete",
                table: "BanTemplate",
                newName: "AutoDelete");

            migrationBuilder.RenameColumn(
                name: "exempt_flags",
                table: "BanTemplate",
                newName: "ExemptFlags");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "blacklist",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "connection_log_id",
                table: "connection_log",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "server_id",
                table: "connection_log",
                newName: "ServerId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "connection_log",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "connection_log",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "hwid_type",
                table: "connection_log",
                newName: "HWId_Type");

            migrationBuilder.RenameTable(
                name: "custom_vote_log",
                newName: "CustomVoteLog");

            migrationBuilder.RenameColumn(
                name: "custom_vote_log_id",
                table: "CustomVoteLog",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "initiator_id",
                table: "CustomVoteLog",
                newName: "InitiatorId");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "CustomVoteLog",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "time_created",
                table: "CustomVoteLog",
                newName: "TimeCreated");

            migrationBuilder.RenameTable(
                name: "custom_vote_log_option",
                newName: "CustomVoteLogOption");

            migrationBuilder.RenameColumn(
                name: "vote_id",
                table: "CustomVoteLogOption",
                newName: "VoteId");

            migrationBuilder.RenameColumn(
                name: "option_idx",
                table: "CustomVoteLogOption",
                newName: "OptionIdx");

            migrationBuilder.RenameColumn(
                name: "vote_count",
                table: "CustomVoteLogOption",
                newName: "VoteCount");

            migrationBuilder.RenameTable(
                name: "ipintel_cache",
                newName: "IPIntelCache");

            migrationBuilder.RenameColumn(
                name: "ipintel_cache_id",
                table: "IPIntelCache",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "job_id",
                table: "job",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "job_name",
                table: "job",
                newName: "JobName");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "job",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "play_time_id",
                table: "play_time",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "player_id",
                table: "play_time",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "time_spent",
                table: "play_time",
                newName: "TimeSpent");

            migrationBuilder.RenameColumn(
                name: "player_id",
                table: "player",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "first_seen_time",
                table: "player",
                newName: "FirstSeenTime");

            migrationBuilder.RenameColumn(
                name: "last_read_rules",
                table: "player",
                newName: "LastReadRules");

            migrationBuilder.RenameColumn(
                name: "last_seen_address",
                table: "player",
                newName: "LastSeenAddress");

            migrationBuilder.RenameColumn(
                name: "last_seen_time",
                table: "player",
                newName: "LastSeenTime");

            migrationBuilder.RenameColumn(
                name: "last_seen_user_name",
                table: "player",
                newName: "LastSeenUserName");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "player",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "last_seen_hwid_type",
                table: "player",
                newName: "LastSeenHWId_Type");

            migrationBuilder.RenameColumn(
                name: "preference_id",
                table: "preference",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "admin_ooc_color",
                table: "preference",
                newName: "AdminOOCColor");

            migrationBuilder.RenameColumn(
                name: "construction_favorites",
                table: "preference",
                newName: "ConstructionFavorites");

            migrationBuilder.RenameColumn(
                name: "selected_character_slot",
                table: "preference",
                newName: "SelectedCharacterSlot");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "preference",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "profile",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "eye_color",
                table: "profile",
                newName: "EyeColor");

            migrationBuilder.RenameColumn(
                name: "facial_hair_color",
                table: "profile",
                newName: "FacialHairColor");

            migrationBuilder.RenameColumn(
                name: "facial_hair_name",
                table: "profile",
                newName: "FacialHairName");

            migrationBuilder.RenameColumn(
                name: "flavor_text",
                table: "profile",
                newName: "FlavorText");

            migrationBuilder.RenameColumn(
                name: "hair_color",
                table: "profile",
                newName: "HairColor");

            migrationBuilder.RenameColumn(
                name: "hair_name",
                table: "profile",
                newName: "HairName");

            migrationBuilder.RenameColumn(
                name: "organ_markings",
                table: "profile",
                newName: "OrganMarkings");

            migrationBuilder.RenameColumn(
                name: "preference_id",
                table: "profile",
                newName: "PreferenceId");

            migrationBuilder.RenameColumn(
                name: "skin_color",
                table: "profile",
                newName: "SkinColor");

            migrationBuilder.RenameColumn(
                name: "spawn_priority",
                table: "profile",
                newName: "SpawnPriority");

            migrationBuilder.RenameTable(
                name: "profile_loadout",
                newName: "ProfileLoadout");

            migrationBuilder.RenameColumn(
                name: "profile_loadout_id",
                table: "ProfileLoadout",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "loadout_name",
                table: "ProfileLoadout",
                newName: "LoadoutName");

            migrationBuilder.RenameColumn(
                name: "profile_loadout_group_id",
                table: "ProfileLoadout",
                newName: "ProfileLoadoutGroupId");

            migrationBuilder.RenameTable(
                name: "profile_loadout_group",
                newName: "ProfileLoadoutGroup");

            migrationBuilder.RenameColumn(
                name: "profile_loadout_group_id",
                table: "ProfileLoadoutGroup",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "group_name",
                table: "ProfileLoadoutGroup",
                newName: "GroupName");

            migrationBuilder.RenameColumn(
                name: "profile_role_loadout_id",
                table: "ProfileLoadoutGroup",
                newName: "ProfileRoleLoadoutId");

            migrationBuilder.RenameTable(
                name: "profile_role_loadout",
                newName: "ProfileRoleLoadout");

            migrationBuilder.RenameColumn(
                name: "profile_role_loadout_id",
                table: "ProfileRoleLoadout",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "entity_name",
                table: "ProfileRoleLoadout",
                newName: "EntityName");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "ProfileRoleLoadout",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "role_name",
                table: "ProfileRoleLoadout",
                newName: "RoleName");

            migrationBuilder.RenameTable(
                name: "role_whitelists",
                newName: "RoleWhitelists");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "RoleWhitelists",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "RoleWhitelists",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "round",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "server_id",
                table: "round",
                newName: "ServerId");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "round",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "server_id",
                table: "server",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "server_ban_exemption",
                newName: "UserId");

            migrationBuilder.RenameTable(
                name: "server_ban_hit",
                newName: "ServerBanHit");

            migrationBuilder.RenameColumn(
                name: "server_ban_hit_id",
                table: "ServerBanHit",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "ServerBanHit",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "connection_id",
                table: "ServerBanHit",
                newName: "ConnectionId");

            migrationBuilder.RenameColumn(
                name: "trait_id",
                table: "trait",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "trait",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "trait_name",
                table: "trait",
                newName: "TraitName");

            migrationBuilder.RenameColumn(
                name: "unban_id",
                table: "unban",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "unban",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "unban_time",
                table: "unban",
                newName: "UnbanTime");

            migrationBuilder.RenameColumn(
                name: "unbanning_admin",
                table: "unban",
                newName: "UnbanningAdmin");

            migrationBuilder.RenameColumn(
                name: "uploaded_resource_log_id",
                table: "uploaded_resource_log",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "uploaded_resource_log",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "whitelist",
                newName: "UserId");
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "whitelist",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "uploaded_resource_log",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "uploaded_resource_log",
                newName: "uploaded_resource_log_id");

            migrationBuilder.RenameColumn(
                name: "UnbanningAdmin",
                table: "unban",
                newName: "unbanning_admin");

            migrationBuilder.RenameColumn(
                name: "UnbanTime",
                table: "unban",
                newName: "unban_time");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "unban",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "unban",
                newName: "unban_id");

            migrationBuilder.RenameColumn(
                name: "TraitName",
                table: "trait",
                newName: "trait_name");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "trait",
                newName: "profile_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "trait",
                newName: "trait_id");

            migrationBuilder.RenameColumn(
                name: "ConnectionId",
                table: "ServerBanHit",
                newName: "connection_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "ServerBanHit",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ServerBanHit",
                newName: "server_ban_hit_id");

            migrationBuilder.RenameTable(
                name: "ServerBanHit",
                newName: "server_ban_hit");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "server_ban_exemption",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "server",
                newName: "server_id");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "round",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "round",
                newName: "server_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "round",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "RoleWhitelists",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "RoleWhitelists",
                newName: "player_user_id");

            migrationBuilder.RenameTable(
                name: "RoleWhitelists",
                newName: "role_whitelists");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "ProfileRoleLoadout",
                newName: "role_name");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "ProfileRoleLoadout",
                newName: "profile_id");

            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "ProfileRoleLoadout",
                newName: "entity_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProfileRoleLoadout",
                newName: "profile_role_loadout_id");

            migrationBuilder.RenameTable(
                name: "ProfileRoleLoadout",
                newName: "profile_role_loadout");

            migrationBuilder.RenameColumn(
                name: "ProfileRoleLoadoutId",
                table: "ProfileLoadoutGroup",
                newName: "profile_role_loadout_id");

            migrationBuilder.RenameColumn(
                name: "GroupName",
                table: "ProfileLoadoutGroup",
                newName: "group_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProfileLoadoutGroup",
                newName: "profile_loadout_group_id");

            migrationBuilder.RenameTable(
                name: "ProfileLoadoutGroup",
                newName: "profile_loadout_group");

            migrationBuilder.RenameColumn(
                name: "ProfileLoadoutGroupId",
                table: "ProfileLoadout",
                newName: "profile_loadout_group_id");

            migrationBuilder.RenameColumn(
                name: "LoadoutName",
                table: "ProfileLoadout",
                newName: "loadout_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProfileLoadout",
                newName: "profile_loadout_id");

            migrationBuilder.RenameTable(
                name: "ProfileLoadout",
                newName: "profile_loadout");

            migrationBuilder.RenameColumn(
                name: "SpawnPriority",
                table: "profile",
                newName: "spawn_priority");

            migrationBuilder.RenameColumn(
                name: "SkinColor",
                table: "profile",
                newName: "skin_color");

            migrationBuilder.RenameColumn(
                name: "PreferenceId",
                table: "profile",
                newName: "preference_id");

            migrationBuilder.RenameColumn(
                name: "OrganMarkings",
                table: "profile",
                newName: "organ_markings");

            migrationBuilder.RenameColumn(
                name: "HairName",
                table: "profile",
                newName: "hair_name");

            migrationBuilder.RenameColumn(
                name: "HairColor",
                table: "profile",
                newName: "hair_color");

            migrationBuilder.RenameColumn(
                name: "FlavorText",
                table: "profile",
                newName: "flavor_text");

            migrationBuilder.RenameColumn(
                name: "FacialHairName",
                table: "profile",
                newName: "facial_hair_name");

            migrationBuilder.RenameColumn(
                name: "FacialHairColor",
                table: "profile",
                newName: "facial_hair_color");

            migrationBuilder.RenameColumn(
                name: "EyeColor",
                table: "profile",
                newName: "eye_color");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "profile",
                newName: "profile_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "preference",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "SelectedCharacterSlot",
                table: "preference",
                newName: "selected_character_slot");

            migrationBuilder.RenameColumn(
                name: "ConstructionFavorites",
                table: "preference",
                newName: "construction_favorites");

            migrationBuilder.RenameColumn(
                name: "AdminOOCColor",
                table: "preference",
                newName: "admin_ooc_color");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "preference",
                newName: "preference_id");

            migrationBuilder.RenameColumn(
                name: "LastSeenHWId_Type",
                table: "player",
                newName: "last_seen_hwid_type");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "player",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "LastSeenUserName",
                table: "player",
                newName: "last_seen_user_name");

            migrationBuilder.RenameColumn(
                name: "LastSeenTime",
                table: "player",
                newName: "last_seen_time");

            migrationBuilder.RenameColumn(
                name: "LastSeenAddress",
                table: "player",
                newName: "last_seen_address");

            migrationBuilder.RenameColumn(
                name: "LastReadRules",
                table: "player",
                newName: "last_read_rules");

            migrationBuilder.RenameColumn(
                name: "FirstSeenTime",
                table: "player",
                newName: "first_seen_time");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "player",
                newName: "player_id");

            migrationBuilder.RenameColumn(
                name: "TimeSpent",
                table: "play_time",
                newName: "time_spent");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "play_time",
                newName: "player_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "play_time",
                newName: "play_time_id");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "job",
                newName: "profile_id");

            migrationBuilder.RenameColumn(
                name: "JobName",
                table: "job",
                newName: "job_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "job",
                newName: "job_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "IPIntelCache",
                newName: "ipintel_cache_id");

            migrationBuilder.RenameTable(
                name: "IPIntelCache",
                newName: "ipintel_cache");

            migrationBuilder.RenameColumn(
                name: "VoteCount",
                table: "CustomVoteLogOption",
                newName: "vote_count");

            migrationBuilder.RenameColumn(
                name: "OptionIdx",
                table: "CustomVoteLogOption",
                newName: "option_idx");

            migrationBuilder.RenameColumn(
                name: "VoteId",
                table: "CustomVoteLogOption",
                newName: "vote_id");

            migrationBuilder.RenameTable(
                name: "CustomVoteLogOption",
                newName: "custom_vote_log_option");

            migrationBuilder.RenameColumn(
                name: "TimeCreated",
                table: "CustomVoteLog",
                newName: "time_created");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "CustomVoteLog",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "InitiatorId",
                table: "CustomVoteLog",
                newName: "initiator_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CustomVoteLog",
                newName: "custom_vote_log_id");

            migrationBuilder.RenameTable(
                name: "CustomVoteLog",
                newName: "custom_vote_log");

            migrationBuilder.RenameColumn(
                name: "HWId_Type",
                table: "connection_log",
                newName: "hwid_type");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "connection_log",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "connection_log",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "connection_log",
                newName: "server_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "connection_log",
                newName: "connection_log_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "blacklist",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ExemptFlags",
                table: "BanTemplate",
                newName: "exempt_flags");

            migrationBuilder.RenameColumn(
                name: "AutoDelete",
                table: "BanTemplate",
                newName: "auto_delete");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BanTemplate",
                newName: "ban_template_id");

            migrationBuilder.RenameTable(
                name: "BanTemplate",
                newName: "ban_template");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "BanRound",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "BanRound",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BanRound",
                newName: "ban_round_id");

            migrationBuilder.RenameTable(
                name: "BanRound",
                newName: "ban_round");

            migrationBuilder.RenameColumn(
                name: "RoleType",
                table: "BanRole",
                newName: "role_type");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "BanRole",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "BanRole",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BanRole",
                newName: "ban_role_id");

            migrationBuilder.RenameTable(
                name: "BanRole",
                newName: "ban_role");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BanPlayer",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "BanPlayer",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BanPlayer",
                newName: "ban_player_id");

            migrationBuilder.RenameTable(
                name: "BanPlayer",
                newName: "ban_player");

            migrationBuilder.RenameColumn(
                name: "HWId_Type",
                table: "BanHwid",
                newName: "hwid_type");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "BanHwid",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BanHwid",
                newName: "ban_hwid_id");

            migrationBuilder.RenameTable(
                name: "BanHwid",
                newName: "ban_hwid");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "BanAddress",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BanAddress",
                newName: "ban_address_id");

            migrationBuilder.RenameTable(
                name: "BanAddress",
                newName: "ban_address");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "ban",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "ban",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "ban",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "ban",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "ExemptFlags",
                table: "ban",
                newName: "exempt_flags");

            migrationBuilder.RenameColumn(
                name: "BanningAdmin",
                table: "ban",
                newName: "banning_admin");

            migrationBuilder.RenameColumn(
                name: "BanTime",
                table: "ban",
                newName: "ban_time");

            migrationBuilder.RenameColumn(
                name: "AutoDelete",
                table: "ban",
                newName: "auto_delete");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ban",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AssignedUserId",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AssignedUserId",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AssignedUserId",
                newName: "assigned_user_id_id");

            migrationBuilder.RenameTable(
                name: "AssignedUserId",
                newName: "assigned_user_id");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "antag",
                newName: "profile_id");

            migrationBuilder.RenameColumn(
                name: "AntagName",
                table: "antag",
                newName: "antag_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "antag",
                newName: "antag_id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminWatchlists",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "AdminWatchlists",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "AdminWatchlists",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "AdminWatchlists",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "AdminWatchlists",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "AdminWatchlists",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "AdminWatchlists",
                newName: "deleted_by_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "AdminWatchlists",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "AdminWatchlists",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AdminWatchlists",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminWatchlists",
                newName: "admin_watchlists_id");

            migrationBuilder.RenameTable(
                name: "AdminWatchlists",
                newName: "admin_watchlists");

            migrationBuilder.RenameColumn(
                name: "AdminRankId",
                table: "AdminRankFlag",
                newName: "admin_rank_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminRankFlag",
                newName: "admin_rank_flag_id");

            migrationBuilder.RenameTable(
                name: "AdminRankFlag",
                newName: "admin_rank_flag");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminRank",
                newName: "admin_rank_id");

            migrationBuilder.RenameTable(
                name: "AdminRank",
                newName: "admin_rank");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminNotes",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "AdminNotes",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "AdminNotes",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "AdminNotes",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "AdminNotes",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "AdminNotes",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "AdminNotes",
                newName: "deleted_by_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "AdminNotes",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "AdminNotes",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AdminNotes",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminNotes",
                newName: "admin_notes_id");

            migrationBuilder.RenameTable(
                name: "AdminNotes",
                newName: "admin_notes");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminMessages",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "AdminMessages",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "AdminMessages",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "AdminMessages",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "AdminMessages",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "AdminMessages",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "AdminMessages",
                newName: "deleted_by_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "AdminMessages",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "AdminMessages",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AdminMessages",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminMessages",
                newName: "admin_messages_id");

            migrationBuilder.RenameTable(
                name: "AdminMessages",
                newName: "admin_messages");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "AdminLogPlayer",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "LogId",
                table: "AdminLogPlayer",
                newName: "log_id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminLogPlayer",
                newName: "round_id");

            migrationBuilder.RenameTable(
                name: "AdminLogPlayer",
                newName: "admin_log_player");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminLog",
                newName: "admin_log_id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminLog",
                newName: "round_id");

            migrationBuilder.RenameTable(
                name: "AdminLog",
                newName: "admin_log");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "AdminFlag",
                newName: "admin_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminFlag",
                newName: "admin_flag_id");

            migrationBuilder.RenameTable(
                name: "AdminFlag",
                newName: "admin_flag");

            migrationBuilder.RenameColumn(
                name: "AdminRankId",
                table: "admin",
                newName: "admin_rank_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "admin",
                newName: "user_id");
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "uploaded_resource_log",
                newName: "uploaded_resource_log_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "uploaded_resource_log",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "server",
                newName: "server_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "player",
                newName: "player_id");

            migrationBuilder.RenameColumn(
                name: "FirstSeenTime",
                table: "player",
                newName: "first_seen_time");

            migrationBuilder.RenameColumn(
                name: "LastReadRules",
                table: "player",
                newName: "last_read_rules");

            migrationBuilder.RenameColumn(
                name: "LastSeenAddress",
                table: "player",
                newName: "last_seen_address");

            migrationBuilder.RenameColumn(
                name: "LastSeenTime",
                table: "player",
                newName: "last_seen_time");

            migrationBuilder.RenameColumn(
                name: "LastSeenUserName",
                table: "player",
                newName: "last_seen_user_name");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "player",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "LastSeenHWId_Type",
                table: "player",
                newName: "last_seen_hwid_type");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "round",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "round",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "round",
                newName: "server_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AssignedUserId",
                newName: "assigned_user_id_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AssignedUserId",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AssignedUserId",
                newName: "user_name");

            migrationBuilder.RenameTable(
                name: "AssignedUserId",
                newName: "assigned_user_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "server_ban_exemption",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ban",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "AutoDelete",
                table: "ban",
                newName: "auto_delete");

            migrationBuilder.RenameColumn(
                name: "BanTime",
                table: "ban",
                newName: "ban_time");

            migrationBuilder.RenameColumn(
                name: "BanningAdmin",
                table: "ban",
                newName: "banning_admin");

            migrationBuilder.RenameColumn(
                name: "ExemptFlags",
                table: "ban",
                newName: "exempt_flags");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "ban",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "ban",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "ban",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "ban",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "unban",
                newName: "unban_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "unban",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "UnbanTime",
                table: "unban",
                newName: "unban_time");

            migrationBuilder.RenameColumn(
                name: "UnbanningAdmin",
                table: "unban",
                newName: "unbanning_admin");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "admin",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "AdminRankId",
                table: "admin",
                newName: "admin_rank_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminRank",
                newName: "admin_rank_id");

            migrationBuilder.RenameTable(
                name: "AdminRank",
                newName: "admin_rank");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminFlag",
                newName: "admin_flag_id");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "AdminFlag",
                newName: "admin_id");

            migrationBuilder.RenameTable(
                name: "AdminFlag",
                newName: "admin_flag");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminRankFlag",
                newName: "admin_rank_flag_id");

            migrationBuilder.RenameColumn(
                name: "AdminRankId",
                table: "AdminRankFlag",
                newName: "admin_rank_id");

            migrationBuilder.RenameTable(
                name: "AdminRankFlag",
                newName: "admin_rank_flag");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "connection_log",
                newName: "connection_log_id");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "connection_log",
                newName: "server_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "connection_log",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "connection_log",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "HWId_Type",
                table: "connection_log",
                newName: "hwid_type");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminWatchlists",
                newName: "admin_watchlists_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AdminWatchlists",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "AdminWatchlists",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "AdminWatchlists",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "AdminWatchlists",
                newName: "deleted_by_id");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "AdminWatchlists",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "AdminWatchlists",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "AdminWatchlists",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "AdminWatchlists",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "AdminWatchlists",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminWatchlists",
                newName: "round_id");

            migrationBuilder.RenameTable(
                name: "AdminWatchlists",
                newName: "admin_watchlists");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "play_time",
                newName: "play_time_id");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "play_time",
                newName: "player_id");

            migrationBuilder.RenameColumn(
                name: "TimeSpent",
                table: "play_time",
                newName: "time_spent");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "RoleWhitelists",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "RoleWhitelists",
                newName: "role_id");

            migrationBuilder.RenameTable(
                name: "RoleWhitelists",
                newName: "role_whitelists");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "whitelist",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminMessages",
                newName: "admin_messages_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AdminMessages",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "AdminMessages",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "AdminMessages",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "AdminMessages",
                newName: "deleted_by_id");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "AdminMessages",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "AdminMessages",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "AdminMessages",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "AdminMessages",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "AdminMessages",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminMessages",
                newName: "round_id");

            migrationBuilder.RenameTable(
                name: "AdminMessages",
                newName: "admin_messages");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "preference",
                newName: "preference_id");

            migrationBuilder.RenameColumn(
                name: "AdminOOCColor",
                table: "preference",
                newName: "admin_ooc_color");

            migrationBuilder.RenameColumn(
                name: "ConstructionFavorites",
                table: "preference",
                newName: "construction_favorites");

            migrationBuilder.RenameColumn(
                name: "SelectedCharacterSlot",
                table: "preference",
                newName: "selected_character_slot");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "preference",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminLog",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminLog",
                newName: "admin_log_id");

            migrationBuilder.RenameTable(
                name: "AdminLog",
                newName: "admin_log");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "AdminLogPlayer",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "LogId",
                table: "AdminLogPlayer",
                newName: "log_id");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "AdminLogPlayer",
                newName: "player_user_id");

            migrationBuilder.RenameTable(
                name: "AdminLogPlayer",
                newName: "admin_log_player");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "profile",
                newName: "profile_id");

            migrationBuilder.RenameColumn(
                name: "EyeColor",
                table: "profile",
                newName: "eye_color");

            migrationBuilder.RenameColumn(
                name: "FacialHairColor",
                table: "profile",
                newName: "facial_hair_color");

            migrationBuilder.RenameColumn(
                name: "FacialHairName",
                table: "profile",
                newName: "facial_hair_name");

            migrationBuilder.RenameColumn(
                name: "FlavorText",
                table: "profile",
                newName: "flavor_text");

            migrationBuilder.RenameColumn(
                name: "HairColor",
                table: "profile",
                newName: "hair_color");

            migrationBuilder.RenameColumn(
                name: "HairName",
                table: "profile",
                newName: "hair_name");

            migrationBuilder.RenameColumn(
                name: "OrganMarkings",
                table: "profile",
                newName: "organ_markings");

            migrationBuilder.RenameColumn(
                name: "PreferenceId",
                table: "profile",
                newName: "preference_id");

            migrationBuilder.RenameColumn(
                name: "SkinColor",
                table: "profile",
                newName: "skin_color");

            migrationBuilder.RenameColumn(
                name: "SpawnPriority",
                table: "profile",
                newName: "spawn_priority");
        }
    }
}
