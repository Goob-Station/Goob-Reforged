using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class MoveToWizdenDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "unban",
                newName: "Unban");

            migrationBuilder.RenameTable(
                name: "trait",
                newName: "Trait");

            migrationBuilder.RenameTable(
                name: "server",
                newName: "Server");

            migrationBuilder.RenameTable(
                name: "round",
                newName: "Round");

            migrationBuilder.RenameTable(
                name: "profile",
                newName: "Profile");

            migrationBuilder.RenameTable(
                name: "preference",
                newName: "Preference");

            migrationBuilder.RenameTable(
                name: "job",
                newName: "Job");

            migrationBuilder.RenameTable(
                name: "ban",
                newName: "Ban");

            migrationBuilder.RenameTable(
                name: "antag",
                newName: "Antag");

            migrationBuilder.RenameTable(
                name: "admin",
                newName: "Admin");

            migrationBuilder.RenameTable(
                name: "server_ban_hit",
                newName: "ServerBanHit");

            migrationBuilder.RenameTable(
                name: "role_whitelists",
                newName: "RoleWhitelists");

            migrationBuilder.RenameTable(
                name: "profile_role_loadout",
                newName: "ProfileRoleLoadout");

            migrationBuilder.RenameTable(
                name: "profile_loadout_group",
                newName: "ProfileLoadoutGroup");

            migrationBuilder.RenameTable(
                name: "profile_loadout",
                newName: "ProfileLoadout");

            migrationBuilder.RenameTable(
                name: "ipintel_cache",
                newName: "IPIntelCache");

            migrationBuilder.RenameTable(
                name: "custom_vote_log_option",
                newName: "CustomVoteLogOption");

            migrationBuilder.RenameTable(
                name: "custom_vote_log",
                newName: "CustomVoteLog");

            migrationBuilder.RenameTable(
                name: "ban_template",
                newName: "BanTemplate");

            migrationBuilder.RenameTable(
                name: "ban_round",
                newName: "BanRound");

            migrationBuilder.RenameTable(
                name: "ban_role",
                newName: "BanRole");

            migrationBuilder.RenameTable(
                name: "ban_player",
                newName: "BanPlayer");

            migrationBuilder.RenameTable(
                name: "ban_hwid",
                newName: "BanHwid");

            migrationBuilder.RenameTable(
                name: "ban_address",
                newName: "BanAddress");

            migrationBuilder.RenameTable(
                name: "assigned_user_id",
                newName: "AssignedUserId");

            migrationBuilder.RenameTable(
                name: "admin_watchlists",
                newName: "AdminWatchlists");

            migrationBuilder.RenameTable(
                name: "admin_rank_flag",
                newName: "AdminRankFlag");

            migrationBuilder.RenameTable(
                name: "admin_rank",
                newName: "AdminRank");

            migrationBuilder.RenameTable(
                name: "admin_notes",
                newName: "AdminNotes");

            migrationBuilder.RenameTable(
                name: "admin_messages",
                newName: "AdminMessages");

            migrationBuilder.RenameTable(
                name: "admin_log_player",
                newName: "AdminLogPlayer");

            migrationBuilder.RenameTable(
                name: "admin_log",
                newName: "AdminLog");

            migrationBuilder.RenameTable(
                name: "admin_flag",
                newName: "AdminFlag");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "whitelist",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "path",
                table: "uploaded_resource_log",
                newName: "Path");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "uploaded_resource_log",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "uploaded_resource_log",
                newName: "Data");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "uploaded_resource_log",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "uploaded_resource_log_id",
                table: "uploaded_resource_log",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "unbanning_admin",
                table: "Unban",
                newName: "UnbanningAdmin");

            migrationBuilder.RenameColumn(
                name: "unban_time",
                table: "Unban",
                newName: "UnbanTime");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "Unban",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "unban_id",
                table: "Unban",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_unban_ban_id",
                table: "Unban",
                newName: "IX_Unban_BanId");

            migrationBuilder.RenameColumn(
                name: "trait_name",
                table: "Trait",
                newName: "TraitName");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "Trait",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "trait_id",
                table: "Trait",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_trait_profile_id_trait_name",
                table: "Trait",
                newName: "IX_Trait_ProfileId_TraitName");

            migrationBuilder.RenameColumn(
                name: "flags",
                table: "server_ban_exemption",
                newName: "Flags");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "server_ban_exemption",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Server",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "server_id",
                table: "Server",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "Round",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "server_id",
                table: "Round",
                newName: "ServerId");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "Round",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_round_start_date",
                table: "Round",
                newName: "IX_Round_StartDate");

            migrationBuilder.RenameIndex(
                name: "IX_round_server_id",
                table: "Round",
                newName: "IX_Round_ServerId");

            migrationBuilder.RenameColumn(
                name: "voice",
                table: "Profile",
                newName: "Voice");

            migrationBuilder.RenameColumn(
                name: "species",
                table: "Profile",
                newName: "Species");

            migrationBuilder.RenameColumn(
                name: "slot",
                table: "Profile",
                newName: "Slot");

            migrationBuilder.RenameColumn(
                name: "sex",
                table: "Profile",
                newName: "Sex");

            migrationBuilder.RenameColumn(
                name: "markings",
                table: "Profile",
                newName: "Markings");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Profile",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "age",
                table: "Profile",
                newName: "Age");

            migrationBuilder.RenameColumn(
                name: "spawn_priority",
                table: "Profile",
                newName: "SpawnPriority");

            migrationBuilder.RenameColumn(
                name: "skin_color",
                table: "Profile",
                newName: "SkinColor");

            migrationBuilder.RenameColumn(
                name: "preference_id",
                table: "Profile",
                newName: "PreferenceId");

            migrationBuilder.RenameColumn(
                name: "organ_markings",
                table: "Profile",
                newName: "OrganMarkings");

            migrationBuilder.RenameColumn(
                name: "hair_name",
                table: "Profile",
                newName: "HairName");

            migrationBuilder.RenameColumn(
                name: "hair_color",
                table: "Profile",
                newName: "HairColor");

            migrationBuilder.RenameColumn(
                name: "flavor_text",
                table: "Profile",
                newName: "FlavorText");

            migrationBuilder.RenameColumn(
                name: "facial_hair_name",
                table: "Profile",
                newName: "FacialHairName");

            migrationBuilder.RenameColumn(
                name: "facial_hair_color",
                table: "Profile",
                newName: "FacialHairColor");

            migrationBuilder.RenameColumn(
                name: "eye_color",
                table: "Profile",
                newName: "EyeColor");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "Profile",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_profile_slot_preference_id",
                table: "Profile",
                newName: "IX_Profile_Slot_PreferenceId");

            migrationBuilder.RenameIndex(
                name: "IX_profile_preference_id",
                table: "Profile",
                newName: "IX_Profile_PreferenceId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Preference",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "selected_character_slot",
                table: "Preference",
                newName: "SelectedCharacterSlot");

            migrationBuilder.RenameColumn(
                name: "construction_favorites",
                table: "Preference",
                newName: "ConstructionFavorites");

            migrationBuilder.RenameColumn(
                name: "admin_ooc_color",
                table: "Preference",
                newName: "AdminOOCColor");

            migrationBuilder.RenameColumn(
                name: "preference_id",
                table: "Preference",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_preference_user_id",
                table: "Preference",
                newName: "IX_Preference_UserId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "player",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "last_seen_user_name",
                table: "player",
                newName: "LastSeenUserName");

            migrationBuilder.RenameColumn(
                name: "last_seen_time",
                table: "player",
                newName: "LastSeenTime");

            migrationBuilder.RenameColumn(
                name: "last_seen_address",
                table: "player",
                newName: "LastSeenAddress");

            migrationBuilder.RenameColumn(
                name: "last_read_rules",
                table: "player",
                newName: "LastReadRules");

            migrationBuilder.RenameColumn(
                name: "first_seen_time",
                table: "player",
                newName: "FirstSeenTime");

            migrationBuilder.RenameColumn(
                name: "player_id",
                table: "player",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "last_seen_hwid_type",
                table: "player",
                newName: "LastSeenHWId_Type");

            migrationBuilder.RenameIndex(
                name: "IX_player_user_id",
                table: "player",
                newName: "IX_player_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_player_last_seen_user_name",
                table: "player",
                newName: "IX_player_LastSeenUserName");

            migrationBuilder.RenameColumn(
                name: "tracker",
                table: "play_time",
                newName: "Tracker");

            migrationBuilder.RenameColumn(
                name: "time_spent",
                table: "play_time",
                newName: "TimeSpent");

            migrationBuilder.RenameColumn(
                name: "player_id",
                table: "play_time",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "play_time_id",
                table: "play_time",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_play_time_player_id_tracker",
                table: "play_time",
                newName: "IX_play_time_PlayerId_Tracker");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "Job",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "Job",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "job_name",
                table: "Job",
                newName: "JobName");

            migrationBuilder.RenameColumn(
                name: "job_id",
                table: "Job",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_job_profile_id_job_name",
                table: "Job",
                newName: "IX_Job_ProfileId_JobName");

            migrationBuilder.RenameIndex(
                name: "IX_job_profile_id",
                table: "Job",
                newName: "IX_Job_ProfileId");

            migrationBuilder.RenameColumn(
                name: "trust",
                table: "connection_log",
                newName: "Trust");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "connection_log",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "hwid_type",
                table: "connection_log",
                newName: "HWId_Type");

            migrationBuilder.RenameColumn(
                name: "denied",
                table: "connection_log",
                newName: "Denied");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "connection_log",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "connection_log",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "connection_log",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "server_id",
                table: "connection_log",
                newName: "ServerId");

            migrationBuilder.RenameColumn(
                name: "connection_log_id",
                table: "connection_log",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_connection_log_time",
                table: "connection_log",
                newName: "IX_connection_log_Time");

            migrationBuilder.RenameIndex(
                name: "IX_connection_log_user_id",
                table: "connection_log",
                newName: "IX_connection_log_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_connection_log_server_id",
                table: "connection_log",
                newName: "IX_connection_log_ServerId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "blacklist",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Ban",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "severity",
                table: "Ban",
                newName: "Severity");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "Ban",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "hidden",
                table: "Ban",
                newName: "Hidden");

            migrationBuilder.RenameColumn(
                name: "playtime_at_note",
                table: "Ban",
                newName: "PlaytimeAtNote");

            migrationBuilder.RenameColumn(
                name: "last_edited_by_id",
                table: "Ban",
                newName: "LastEditedById");

            migrationBuilder.RenameColumn(
                name: "last_edited_at",
                table: "Ban",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "expiration_time",
                table: "Ban",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "exempt_flags",
                table: "Ban",
                newName: "ExemptFlags");

            migrationBuilder.RenameColumn(
                name: "banning_admin",
                table: "Ban",
                newName: "BanningAdmin");

            migrationBuilder.RenameColumn(
                name: "ban_time",
                table: "Ban",
                newName: "BanTime");

            migrationBuilder.RenameColumn(
                name: "auto_delete",
                table: "Ban",
                newName: "AutoDelete");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "Ban",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ban_last_edited_by_id",
                table: "Ban",
                newName: "IX_Ban_LastEditedById");

            migrationBuilder.RenameIndex(
                name: "IX_ban_banning_admin",
                table: "Ban",
                newName: "IX_Ban_BanningAdmin");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "Antag",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "antag_name",
                table: "Antag",
                newName: "AntagName");

            migrationBuilder.RenameColumn(
                name: "antag_id",
                table: "Antag",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_antag_profile_id_antag_name",
                table: "Antag",
                newName: "IX_Antag_ProfileId_AntagName");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Admin",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "suspended",
                table: "Admin",
                newName: "Suspended");

            migrationBuilder.RenameColumn(
                name: "deadminned",
                table: "Admin",
                newName: "Deadminned");

            migrationBuilder.RenameColumn(
                name: "admin_rank_id",
                table: "Admin",
                newName: "AdminRankId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Admin",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_admin_rank_id",
                table: "Admin",
                newName: "IX_Admin_AdminRankId");

            migrationBuilder.RenameColumn(
                name: "connection_id",
                table: "ServerBanHit",
                newName: "ConnectionId");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "ServerBanHit",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "server_ban_hit_id",
                table: "ServerBanHit",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_server_ban_hit_connection_id",
                table: "ServerBanHit",
                newName: "IX_ServerBanHit_ConnectionId");

            migrationBuilder.RenameIndex(
                name: "IX_server_ban_hit_ban_id",
                table: "ServerBanHit",
                newName: "IX_ServerBanHit_BanId");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "RoleWhitelists",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "RoleWhitelists",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "role_name",
                table: "ProfileRoleLoadout",
                newName: "RoleName");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "ProfileRoleLoadout",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "entity_name",
                table: "ProfileRoleLoadout",
                newName: "EntityName");

            migrationBuilder.RenameColumn(
                name: "profile_role_loadout_id",
                table: "ProfileRoleLoadout",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_profile_role_loadout_profile_id",
                table: "ProfileRoleLoadout",
                newName: "IX_ProfileRoleLoadout_ProfileId");

            migrationBuilder.RenameColumn(
                name: "profile_role_loadout_id",
                table: "ProfileLoadoutGroup",
                newName: "ProfileRoleLoadoutId");

            migrationBuilder.RenameColumn(
                name: "group_name",
                table: "ProfileLoadoutGroup",
                newName: "GroupName");

            migrationBuilder.RenameColumn(
                name: "profile_loadout_group_id",
                table: "ProfileLoadoutGroup",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_profile_loadout_group_profile_role_loadout_id",
                table: "ProfileLoadoutGroup",
                newName: "IX_ProfileLoadoutGroup_ProfileRoleLoadoutId");

            migrationBuilder.RenameColumn(
                name: "profile_loadout_group_id",
                table: "ProfileLoadout",
                newName: "ProfileLoadoutGroupId");

            migrationBuilder.RenameColumn(
                name: "loadout_name",
                table: "ProfileLoadout",
                newName: "LoadoutName");

            migrationBuilder.RenameColumn(
                name: "profile_loadout_id",
                table: "ProfileLoadout",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_profile_loadout_profile_loadout_group_id",
                table: "ProfileLoadout",
                newName: "IX_ProfileLoadout_ProfileLoadoutGroupId");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "IPIntelCache",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "score",
                table: "IPIntelCache",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "IPIntelCache",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "ipintel_cache_id",
                table: "IPIntelCache",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "text",
                table: "CustomVoteLogOption",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "vote_count",
                table: "CustomVoteLogOption",
                newName: "VoteCount");

            migrationBuilder.RenameColumn(
                name: "option_idx",
                table: "CustomVoteLogOption",
                newName: "OptionIdx");

            migrationBuilder.RenameColumn(
                name: "vote_id",
                table: "CustomVoteLogOption",
                newName: "VoteId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "CustomVoteLog",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "CustomVoteLog",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "time_created",
                table: "CustomVoteLog",
                newName: "TimeCreated");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "CustomVoteLog",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "initiator_id",
                table: "CustomVoteLog",
                newName: "InitiatorId");

            migrationBuilder.RenameColumn(
                name: "custom_vote_log_id",
                table: "CustomVoteLog",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_custom_vote_log_round_id",
                table: "CustomVoteLog",
                newName: "IX_CustomVoteLog_RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_custom_vote_log_initiator_id",
                table: "CustomVoteLog",
                newName: "IX_CustomVoteLog_InitiatorId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "BanTemplate",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "severity",
                table: "BanTemplate",
                newName: "Severity");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "BanTemplate",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "length",
                table: "BanTemplate",
                newName: "Length");

            migrationBuilder.RenameColumn(
                name: "hidden",
                table: "BanTemplate",
                newName: "Hidden");

            migrationBuilder.RenameColumn(
                name: "exempt_flags",
                table: "BanTemplate",
                newName: "ExemptFlags");

            migrationBuilder.RenameColumn(
                name: "auto_delete",
                table: "BanTemplate",
                newName: "AutoDelete");

            migrationBuilder.RenameColumn(
                name: "ban_template_id",
                table: "BanTemplate",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "BanRound",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanRound",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "ban_round_id",
                table: "BanRound",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ban_round_round_id_ban_id",
                table: "BanRound",
                newName: "IX_BanRound_RoundId_BanId");

            migrationBuilder.RenameIndex(
                name: "IX_ban_round_ban_id",
                table: "BanRound",
                newName: "IX_BanRound_BanId");

            migrationBuilder.RenameColumn(
                name: "role_type",
                table: "BanRole",
                newName: "RoleType");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "BanRole",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanRole",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "ban_role_id",
                table: "BanRole",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ban_role_role_type_role_id_ban_id",
                table: "BanRole",
                newName: "IX_BanRole_RoleType_RoleId_BanId");

            migrationBuilder.RenameIndex(
                name: "IX_ban_role_ban_id",
                table: "BanRole",
                newName: "IX_BanRole_BanId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "BanPlayer",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanPlayer",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "ban_player_id",
                table: "BanPlayer",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ban_player_user_id_ban_id",
                table: "BanPlayer",
                newName: "IX_BanPlayer_UserId_BanId");

            migrationBuilder.RenameIndex(
                name: "IX_ban_player_ban_id",
                table: "BanPlayer",
                newName: "IX_BanPlayer_BanId");

            migrationBuilder.RenameColumn(
                name: "hwid_type",
                table: "BanHwid",
                newName: "HWId_Type");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanHwid",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "ban_hwid_id",
                table: "BanHwid",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ban_hwid_ban_id",
                table: "BanHwid",
                newName: "IX_BanHwid_BanId");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "BanAddress",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "ban_id",
                table: "BanAddress",
                newName: "BanId");

            migrationBuilder.RenameColumn(
                name: "ban_address_id",
                table: "BanAddress",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ban_address_ban_id",
                table: "BanAddress",
                newName: "IX_BanAddress_BanId");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "AssignedUserId",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AssignedUserId",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "assigned_user_id_id",
                table: "AssignedUserId",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_assigned_user_id_user_name",
                table: "AssignedUserId",
                newName: "IX_AssignedUserId_UserName");

            migrationBuilder.RenameIndex(
                name: "IX_assigned_user_id_user_id",
                table: "AssignedUserId",
                newName: "IX_AssignedUserId_UserId");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "AdminWatchlists",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "deleted",
                table: "AdminWatchlists",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminWatchlists",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "playtime_at_note",
                table: "AdminWatchlists",
                newName: "PlaytimeAtNote");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "AdminWatchlists",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "last_edited_by_id",
                table: "AdminWatchlists",
                newName: "LastEditedById");

            migrationBuilder.RenameColumn(
                name: "last_edited_at",
                table: "AdminWatchlists",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "expiration_time",
                table: "AdminWatchlists",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "deleted_by_id",
                table: "AdminWatchlists",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "AdminWatchlists",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "AdminWatchlists",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AdminWatchlists",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "admin_watchlists_id",
                table: "AdminWatchlists",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_admin_watchlists_round_id",
                table: "AdminWatchlists",
                newName: "IX_AdminWatchlists_RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_watchlists_player_user_id",
                table: "AdminWatchlists",
                newName: "IX_AdminWatchlists_PlayerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_watchlists_last_edited_by_id",
                table: "AdminWatchlists",
                newName: "IX_AdminWatchlists_LastEditedById");

            migrationBuilder.RenameIndex(
                name: "IX_admin_watchlists_deleted_by_id",
                table: "AdminWatchlists",
                newName: "IX_AdminWatchlists_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_admin_watchlists_created_by_id",
                table: "AdminWatchlists",
                newName: "IX_AdminWatchlists_CreatedById");

            migrationBuilder.RenameColumn(
                name: "flag",
                table: "AdminRankFlag",
                newName: "Flag");

            migrationBuilder.RenameColumn(
                name: "admin_rank_id",
                table: "AdminRankFlag",
                newName: "AdminRankId");

            migrationBuilder.RenameColumn(
                name: "admin_rank_flag_id",
                table: "AdminRankFlag",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_admin_rank_flag_flag_admin_rank_id",
                table: "AdminRankFlag",
                newName: "IX_AdminRankFlag_Flag_AdminRankId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_rank_flag_admin_rank_id",
                table: "AdminRankFlag",
                newName: "IX_AdminRankFlag_AdminRankId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AdminRank",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "admin_rank_id",
                table: "AdminRank",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "severity",
                table: "AdminNotes",
                newName: "Severity");

            migrationBuilder.RenameColumn(
                name: "secret",
                table: "AdminNotes",
                newName: "Secret");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "AdminNotes",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "deleted",
                table: "AdminNotes",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminNotes",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "playtime_at_note",
                table: "AdminNotes",
                newName: "PlaytimeAtNote");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "AdminNotes",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "last_edited_by_id",
                table: "AdminNotes",
                newName: "LastEditedById");

            migrationBuilder.RenameColumn(
                name: "last_edited_at",
                table: "AdminNotes",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "expiration_time",
                table: "AdminNotes",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "deleted_by_id",
                table: "AdminNotes",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "AdminNotes",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "AdminNotes",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AdminNotes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "admin_notes_id",
                table: "AdminNotes",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_admin_notes_round_id",
                table: "AdminNotes",
                newName: "IX_AdminNotes_RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_notes_player_user_id",
                table: "AdminNotes",
                newName: "IX_AdminNotes_PlayerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_notes_last_edited_by_id",
                table: "AdminNotes",
                newName: "IX_AdminNotes_LastEditedById");

            migrationBuilder.RenameIndex(
                name: "IX_admin_notes_deleted_by_id",
                table: "AdminNotes",
                newName: "IX_AdminNotes_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_admin_notes_created_by_id",
                table: "AdminNotes",
                newName: "IX_AdminNotes_CreatedById");

            migrationBuilder.RenameColumn(
                name: "seen",
                table: "AdminMessages",
                newName: "Seen");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "AdminMessages",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "dismissed",
                table: "AdminMessages",
                newName: "Dismissed");

            migrationBuilder.RenameColumn(
                name: "deleted",
                table: "AdminMessages",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminMessages",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "playtime_at_note",
                table: "AdminMessages",
                newName: "PlaytimeAtNote");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "AdminMessages",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "last_edited_by_id",
                table: "AdminMessages",
                newName: "LastEditedById");

            migrationBuilder.RenameColumn(
                name: "last_edited_at",
                table: "AdminMessages",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "expiration_time",
                table: "AdminMessages",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "deleted_by_id",
                table: "AdminMessages",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "AdminMessages",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "AdminMessages",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AdminMessages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "admin_messages_id",
                table: "AdminMessages",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_admin_messages_round_id",
                table: "AdminMessages",
                newName: "IX_AdminMessages_RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_messages_player_user_id",
                table: "AdminMessages",
                newName: "IX_AdminMessages_PlayerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_messages_last_edited_by_id",
                table: "AdminMessages",
                newName: "IX_AdminMessages_LastEditedById");

            migrationBuilder.RenameIndex(
                name: "IX_admin_messages_deleted_by_id",
                table: "AdminMessages",
                newName: "IX_AdminMessages_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_admin_messages_created_by_id",
                table: "AdminMessages",
                newName: "IX_AdminMessages_CreatedById");

            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "AdminLogPlayer",
                newName: "PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "log_id",
                table: "AdminLogPlayer",
                newName: "LogId");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminLogPlayer",
                newName: "RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_log_player_player_user_id",
                table: "AdminLogPlayer",
                newName: "IX_AdminLogPlayer_PlayerUserId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "AdminLog",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "AdminLog",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "json",
                table: "AdminLog",
                newName: "Json");

            migrationBuilder.RenameColumn(
                name: "impact",
                table: "AdminLog",
                newName: "Impact");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "AdminLog",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "admin_log_id",
                table: "AdminLog",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "AdminLog",
                newName: "RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_log_type",
                table: "AdminLog",
                newName: "IX_AdminLog_Type");

            migrationBuilder.RenameIndex(
                name: "IX_admin_log_message",
                table: "AdminLog",
                newName: "IX_AdminLog_Message");

            migrationBuilder.RenameIndex(
                name: "IX_admin_log_date",
                table: "AdminLog",
                newName: "IX_AdminLog_Date");

            migrationBuilder.RenameColumn(
                name: "negative",
                table: "AdminFlag",
                newName: "Negative");

            migrationBuilder.RenameColumn(
                name: "flag",
                table: "AdminFlag",
                newName: "Flag");

            migrationBuilder.RenameColumn(
                name: "admin_id",
                table: "AdminFlag",
                newName: "AdminId");

            migrationBuilder.RenameColumn(
                name: "admin_flag_id",
                table: "AdminFlag",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_admin_flag_flag_admin_id",
                table: "AdminFlag",
                newName: "IX_AdminFlag_Flag_AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_admin_flag_admin_id",
                table: "AdminFlag",
                newName: "IX_AdminFlag_AdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Unban",
                newName: "unban");

            migrationBuilder.RenameTable(
                name: "Trait",
                newName: "trait");

            migrationBuilder.RenameTable(
                name: "Server",
                newName: "server");

            migrationBuilder.RenameTable(
                name: "Round",
                newName: "round");

            migrationBuilder.RenameTable(
                name: "Profile",
                newName: "profile");

            migrationBuilder.RenameTable(
                name: "Preference",
                newName: "preference");

            migrationBuilder.RenameTable(
                name: "Job",
                newName: "job");

            migrationBuilder.RenameTable(
                name: "Ban",
                newName: "ban");

            migrationBuilder.RenameTable(
                name: "Antag",
                newName: "antag");

            migrationBuilder.RenameTable(
                name: "Admin",
                newName: "admin");

            migrationBuilder.RenameTable(
                name: "ServerBanHit",
                newName: "server_ban_hit");

            migrationBuilder.RenameTable(
                name: "RoleWhitelists",
                newName: "role_whitelists");

            migrationBuilder.RenameTable(
                name: "ProfileRoleLoadout",
                newName: "profile_role_loadout");

            migrationBuilder.RenameTable(
                name: "ProfileLoadoutGroup",
                newName: "profile_loadout_group");

            migrationBuilder.RenameTable(
                name: "ProfileLoadout",
                newName: "profile_loadout");

            migrationBuilder.RenameTable(
                name: "IPIntelCache",
                newName: "ipintel_cache");

            migrationBuilder.RenameTable(
                name: "CustomVoteLogOption",
                newName: "custom_vote_log_option");

            migrationBuilder.RenameTable(
                name: "CustomVoteLog",
                newName: "custom_vote_log");

            migrationBuilder.RenameTable(
                name: "BanTemplate",
                newName: "ban_template");

            migrationBuilder.RenameTable(
                name: "BanRound",
                newName: "ban_round");

            migrationBuilder.RenameTable(
                name: "BanRole",
                newName: "ban_role");

            migrationBuilder.RenameTable(
                name: "BanPlayer",
                newName: "ban_player");

            migrationBuilder.RenameTable(
                name: "BanHwid",
                newName: "ban_hwid");

            migrationBuilder.RenameTable(
                name: "BanAddress",
                newName: "ban_address");

            migrationBuilder.RenameTable(
                name: "AssignedUserId",
                newName: "assigned_user_id");

            migrationBuilder.RenameTable(
                name: "AdminWatchlists",
                newName: "admin_watchlists");

            migrationBuilder.RenameTable(
                name: "AdminRankFlag",
                newName: "admin_rank_flag");

            migrationBuilder.RenameTable(
                name: "AdminRank",
                newName: "admin_rank");

            migrationBuilder.RenameTable(
                name: "AdminNotes",
                newName: "admin_notes");

            migrationBuilder.RenameTable(
                name: "AdminMessages",
                newName: "admin_messages");

            migrationBuilder.RenameTable(
                name: "AdminLogPlayer",
                newName: "admin_log_player");

            migrationBuilder.RenameTable(
                name: "AdminLog",
                newName: "admin_log");

            migrationBuilder.RenameTable(
                name: "AdminFlag",
                newName: "admin_flag");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "whitelist",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "uploaded_resource_log",
                newName: "path");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "uploaded_resource_log",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "uploaded_resource_log",
                newName: "data");

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

            migrationBuilder.RenameIndex(
                name: "IX_Unban_BanId",
                table: "unban",
                newName: "IX_unban_ban_id");

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

            migrationBuilder.RenameIndex(
                name: "IX_Trait_ProfileId_TraitName",
                table: "trait",
                newName: "IX_trait_profile_id_trait_name");

            migrationBuilder.RenameColumn(
                name: "Flags",
                table: "server_ban_exemption",
                newName: "flags");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "server_ban_exemption",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "server",
                newName: "name");

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

            migrationBuilder.RenameIndex(
                name: "IX_Round_StartDate",
                table: "round",
                newName: "IX_round_start_date");

            migrationBuilder.RenameIndex(
                name: "IX_Round_ServerId",
                table: "round",
                newName: "IX_round_server_id");

            migrationBuilder.RenameColumn(
                name: "Voice",
                table: "profile",
                newName: "voice");

            migrationBuilder.RenameColumn(
                name: "Species",
                table: "profile",
                newName: "species");

            migrationBuilder.RenameColumn(
                name: "Slot",
                table: "profile",
                newName: "slot");

            migrationBuilder.RenameColumn(
                name: "Sex",
                table: "profile",
                newName: "sex");

            migrationBuilder.RenameColumn(
                name: "Markings",
                table: "profile",
                newName: "markings");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "profile",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "profile",
                newName: "age");

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

            migrationBuilder.RenameIndex(
                name: "IX_Profile_Slot_PreferenceId",
                table: "profile",
                newName: "IX_profile_slot_preference_id");

            migrationBuilder.RenameIndex(
                name: "IX_Profile_PreferenceId",
                table: "profile",
                newName: "IX_profile_preference_id");

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

            migrationBuilder.RenameIndex(
                name: "IX_Preference_UserId",
                table: "preference",
                newName: "IX_preference_user_id");

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
                name: "LastSeenHWId_Type",
                table: "player",
                newName: "last_seen_hwid_type");

            migrationBuilder.RenameIndex(
                name: "IX_player_UserId",
                table: "player",
                newName: "IX_player_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_LastSeenUserName",
                table: "player",
                newName: "IX_player_last_seen_user_name");

            migrationBuilder.RenameColumn(
                name: "Tracker",
                table: "play_time",
                newName: "tracker");

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

            migrationBuilder.RenameIndex(
                name: "IX_play_time_PlayerId_Tracker",
                table: "play_time",
                newName: "IX_play_time_player_id_tracker");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "job",
                newName: "priority");

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

            migrationBuilder.RenameIndex(
                name: "IX_Job_ProfileId_JobName",
                table: "job",
                newName: "IX_job_profile_id_job_name");

            migrationBuilder.RenameIndex(
                name: "IX_Job_ProfileId",
                table: "job",
                newName: "IX_job_profile_id");

            migrationBuilder.RenameColumn(
                name: "Trust",
                table: "connection_log",
                newName: "trust");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "connection_log",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "HWId_Type",
                table: "connection_log",
                newName: "hwid_type");

            migrationBuilder.RenameColumn(
                name: "Denied",
                table: "connection_log",
                newName: "denied");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "connection_log",
                newName: "address");

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

            migrationBuilder.RenameIndex(
                name: "IX_connection_log_Time",
                table: "connection_log",
                newName: "IX_connection_log_time");

            migrationBuilder.RenameIndex(
                name: "IX_connection_log_UserId",
                table: "connection_log",
                newName: "IX_connection_log_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_connection_log_ServerId",
                table: "connection_log",
                newName: "IX_connection_log_server_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "blacklist",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ban",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Severity",
                table: "ban",
                newName: "severity");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "ban",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "Hidden",
                table: "ban",
                newName: "hidden");

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

            migrationBuilder.RenameIndex(
                name: "IX_Ban_LastEditedById",
                table: "ban",
                newName: "IX_ban_last_edited_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_Ban_BanningAdmin",
                table: "ban",
                newName: "IX_ban_banning_admin");

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

            migrationBuilder.RenameIndex(
                name: "IX_Antag_ProfileId_AntagName",
                table: "antag",
                newName: "IX_antag_profile_id_antag_name");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "admin",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Suspended",
                table: "admin",
                newName: "suspended");

            migrationBuilder.RenameColumn(
                name: "Deadminned",
                table: "admin",
                newName: "deadminned");

            migrationBuilder.RenameColumn(
                name: "AdminRankId",
                table: "admin",
                newName: "admin_rank_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "admin",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Admin_AdminRankId",
                table: "admin",
                newName: "IX_admin_admin_rank_id");

            migrationBuilder.RenameColumn(
                name: "ConnectionId",
                table: "server_ban_hit",
                newName: "connection_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "server_ban_hit",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "server_ban_hit",
                newName: "server_ban_hit_id");

            migrationBuilder.RenameIndex(
                name: "IX_ServerBanHit_ConnectionId",
                table: "server_ban_hit",
                newName: "IX_server_ban_hit_connection_id");

            migrationBuilder.RenameIndex(
                name: "IX_ServerBanHit_BanId",
                table: "server_ban_hit",
                newName: "IX_server_ban_hit_ban_id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "role_whitelists",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "role_whitelists",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "profile_role_loadout",
                newName: "role_name");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "profile_role_loadout",
                newName: "profile_id");

            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "profile_role_loadout",
                newName: "entity_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "profile_role_loadout",
                newName: "profile_role_loadout_id");

            migrationBuilder.RenameIndex(
                name: "IX_ProfileRoleLoadout_ProfileId",
                table: "profile_role_loadout",
                newName: "IX_profile_role_loadout_profile_id");

            migrationBuilder.RenameColumn(
                name: "ProfileRoleLoadoutId",
                table: "profile_loadout_group",
                newName: "profile_role_loadout_id");

            migrationBuilder.RenameColumn(
                name: "GroupName",
                table: "profile_loadout_group",
                newName: "group_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "profile_loadout_group",
                newName: "profile_loadout_group_id");

            migrationBuilder.RenameIndex(
                name: "IX_ProfileLoadoutGroup_ProfileRoleLoadoutId",
                table: "profile_loadout_group",
                newName: "IX_profile_loadout_group_profile_role_loadout_id");

            migrationBuilder.RenameColumn(
                name: "ProfileLoadoutGroupId",
                table: "profile_loadout",
                newName: "profile_loadout_group_id");

            migrationBuilder.RenameColumn(
                name: "LoadoutName",
                table: "profile_loadout",
                newName: "loadout_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "profile_loadout",
                newName: "profile_loadout_id");

            migrationBuilder.RenameIndex(
                name: "IX_ProfileLoadout_ProfileLoadoutGroupId",
                table: "profile_loadout",
                newName: "IX_profile_loadout_profile_loadout_group_id");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "ipintel_cache",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "ipintel_cache",
                newName: "score");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "ipintel_cache",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ipintel_cache",
                newName: "ipintel_cache_id");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "custom_vote_log_option",
                newName: "text");

            migrationBuilder.RenameColumn(
                name: "VoteCount",
                table: "custom_vote_log_option",
                newName: "vote_count");

            migrationBuilder.RenameColumn(
                name: "OptionIdx",
                table: "custom_vote_log_option",
                newName: "option_idx");

            migrationBuilder.RenameColumn(
                name: "VoteId",
                table: "custom_vote_log_option",
                newName: "vote_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "custom_vote_log",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "custom_vote_log",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "TimeCreated",
                table: "custom_vote_log",
                newName: "time_created");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "custom_vote_log",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "InitiatorId",
                table: "custom_vote_log",
                newName: "initiator_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "custom_vote_log",
                newName: "custom_vote_log_id");

            migrationBuilder.RenameIndex(
                name: "IX_CustomVoteLog_RoundId",
                table: "custom_vote_log",
                newName: "IX_custom_vote_log_round_id");

            migrationBuilder.RenameIndex(
                name: "IX_CustomVoteLog_InitiatorId",
                table: "custom_vote_log",
                newName: "IX_custom_vote_log_initiator_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ban_template",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Severity",
                table: "ban_template",
                newName: "severity");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "ban_template",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "Length",
                table: "ban_template",
                newName: "length");

            migrationBuilder.RenameColumn(
                name: "Hidden",
                table: "ban_template",
                newName: "hidden");

            migrationBuilder.RenameColumn(
                name: "ExemptFlags",
                table: "ban_template",
                newName: "exempt_flags");

            migrationBuilder.RenameColumn(
                name: "AutoDelete",
                table: "ban_template",
                newName: "auto_delete");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ban_template",
                newName: "ban_template_id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "ban_round",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "ban_round",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ban_round",
                newName: "ban_round_id");

            migrationBuilder.RenameIndex(
                name: "IX_BanRound_RoundId_BanId",
                table: "ban_round",
                newName: "IX_ban_round_round_id_ban_id");

            migrationBuilder.RenameIndex(
                name: "IX_BanRound_BanId",
                table: "ban_round",
                newName: "IX_ban_round_ban_id");

            migrationBuilder.RenameColumn(
                name: "RoleType",
                table: "ban_role",
                newName: "role_type");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "ban_role",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "ban_role",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ban_role",
                newName: "ban_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_BanRole_RoleType_RoleId_BanId",
                table: "ban_role",
                newName: "IX_ban_role_role_type_role_id_ban_id");

            migrationBuilder.RenameIndex(
                name: "IX_BanRole_BanId",
                table: "ban_role",
                newName: "IX_ban_role_ban_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ban_player",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "ban_player",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ban_player",
                newName: "ban_player_id");

            migrationBuilder.RenameIndex(
                name: "IX_BanPlayer_UserId_BanId",
                table: "ban_player",
                newName: "IX_ban_player_user_id_ban_id");

            migrationBuilder.RenameIndex(
                name: "IX_BanPlayer_BanId",
                table: "ban_player",
                newName: "IX_ban_player_ban_id");

            migrationBuilder.RenameColumn(
                name: "HWId_Type",
                table: "ban_hwid",
                newName: "hwid_type");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "ban_hwid",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ban_hwid",
                newName: "ban_hwid_id");

            migrationBuilder.RenameIndex(
                name: "IX_BanHwid_BanId",
                table: "ban_hwid",
                newName: "IX_ban_hwid_ban_id");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "ban_address",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "ban_address",
                newName: "ban_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ban_address",
                newName: "ban_address_id");

            migrationBuilder.RenameIndex(
                name: "IX_BanAddress_BanId",
                table: "ban_address",
                newName: "IX_ban_address_ban_id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "assigned_user_id",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "assigned_user_id",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "assigned_user_id",
                newName: "assigned_user_id_id");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedUserId_UserName",
                table: "assigned_user_id",
                newName: "IX_assigned_user_id_user_name");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedUserId_UserId",
                table: "assigned_user_id",
                newName: "IX_assigned_user_id_user_id");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "admin_watchlists",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "admin_watchlists",
                newName: "deleted");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "admin_watchlists",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "admin_watchlists",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "admin_watchlists",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "admin_watchlists",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "admin_watchlists",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "admin_watchlists",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "admin_watchlists",
                newName: "deleted_by_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "admin_watchlists",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "admin_watchlists",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "admin_watchlists",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_watchlists",
                newName: "admin_watchlists_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminWatchlists_RoundId",
                table: "admin_watchlists",
                newName: "IX_admin_watchlists_round_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminWatchlists_PlayerUserId",
                table: "admin_watchlists",
                newName: "IX_admin_watchlists_player_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminWatchlists_LastEditedById",
                table: "admin_watchlists",
                newName: "IX_admin_watchlists_last_edited_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminWatchlists_DeletedById",
                table: "admin_watchlists",
                newName: "IX_admin_watchlists_deleted_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminWatchlists_CreatedById",
                table: "admin_watchlists",
                newName: "IX_admin_watchlists_created_by_id");

            migrationBuilder.RenameColumn(
                name: "Flag",
                table: "admin_rank_flag",
                newName: "flag");

            migrationBuilder.RenameColumn(
                name: "AdminRankId",
                table: "admin_rank_flag",
                newName: "admin_rank_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_rank_flag",
                newName: "admin_rank_flag_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminRankFlag_Flag_AdminRankId",
                table: "admin_rank_flag",
                newName: "IX_admin_rank_flag_flag_admin_rank_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminRankFlag_AdminRankId",
                table: "admin_rank_flag",
                newName: "IX_admin_rank_flag_admin_rank_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "admin_rank",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_rank",
                newName: "admin_rank_id");

            migrationBuilder.RenameColumn(
                name: "Severity",
                table: "admin_notes",
                newName: "severity");

            migrationBuilder.RenameColumn(
                name: "Secret",
                table: "admin_notes",
                newName: "secret");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "admin_notes",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "admin_notes",
                newName: "deleted");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "admin_notes",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "admin_notes",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "admin_notes",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "admin_notes",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "admin_notes",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "admin_notes",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "admin_notes",
                newName: "deleted_by_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "admin_notes",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "admin_notes",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "admin_notes",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_notes",
                newName: "admin_notes_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminNotes_RoundId",
                table: "admin_notes",
                newName: "IX_admin_notes_round_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminNotes_PlayerUserId",
                table: "admin_notes",
                newName: "IX_admin_notes_player_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminNotes_LastEditedById",
                table: "admin_notes",
                newName: "IX_admin_notes_last_edited_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminNotes_DeletedById",
                table: "admin_notes",
                newName: "IX_admin_notes_deleted_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminNotes_CreatedById",
                table: "admin_notes",
                newName: "IX_admin_notes_created_by_id");

            migrationBuilder.RenameColumn(
                name: "Seen",
                table: "admin_messages",
                newName: "seen");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "admin_messages",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Dismissed",
                table: "admin_messages",
                newName: "dismissed");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "admin_messages",
                newName: "deleted");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "admin_messages",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "PlaytimeAtNote",
                table: "admin_messages",
                newName: "playtime_at_note");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "admin_messages",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedById",
                table: "admin_messages",
                newName: "last_edited_by_id");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "admin_messages",
                newName: "last_edited_at");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "admin_messages",
                newName: "expiration_time");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "admin_messages",
                newName: "deleted_by_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "admin_messages",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "admin_messages",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "admin_messages",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_messages",
                newName: "admin_messages_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminMessages_RoundId",
                table: "admin_messages",
                newName: "IX_admin_messages_round_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminMessages_PlayerUserId",
                table: "admin_messages",
                newName: "IX_admin_messages_player_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminMessages_LastEditedById",
                table: "admin_messages",
                newName: "IX_admin_messages_last_edited_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminMessages_DeletedById",
                table: "admin_messages",
                newName: "IX_admin_messages_deleted_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminMessages_CreatedById",
                table: "admin_messages",
                newName: "IX_admin_messages_created_by_id");

            migrationBuilder.RenameColumn(
                name: "PlayerUserId",
                table: "admin_log_player",
                newName: "player_user_id");

            migrationBuilder.RenameColumn(
                name: "LogId",
                table: "admin_log_player",
                newName: "log_id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "admin_log_player",
                newName: "round_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminLogPlayer_PlayerUserId",
                table: "admin_log_player",
                newName: "IX_admin_log_player_player_user_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "admin_log",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "admin_log",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Json",
                table: "admin_log",
                newName: "json");

            migrationBuilder.RenameColumn(
                name: "Impact",
                table: "admin_log",
                newName: "impact");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "admin_log",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_log",
                newName: "admin_log_id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "admin_log",
                newName: "round_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminLog_Type",
                table: "admin_log",
                newName: "IX_admin_log_type");

            migrationBuilder.RenameIndex(
                name: "IX_AdminLog_Message",
                table: "admin_log",
                newName: "IX_admin_log_message");

            migrationBuilder.RenameIndex(
                name: "IX_AdminLog_Date",
                table: "admin_log",
                newName: "IX_admin_log_date");

            migrationBuilder.RenameColumn(
                name: "Negative",
                table: "admin_flag",
                newName: "negative");

            migrationBuilder.RenameColumn(
                name: "Flag",
                table: "admin_flag",
                newName: "flag");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "admin_flag",
                newName: "admin_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_flag",
                newName: "admin_flag_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminFlag_Flag_AdminId",
                table: "admin_flag",
                newName: "IX_admin_flag_flag_admin_id");

            migrationBuilder.RenameIndex(
                name: "IX_AdminFlag_AdminId",
                table: "admin_flag",
                newName: "IX_admin_flag_admin_id");
        }
    }
}
