using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ss14");

            migrationBuilder.CreateTable(
                name: "admin_rank",
                schema: "ss14",
                columns: table => new
                {
                    admin_rank_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_rank", x => x.admin_rank_id);
                });

            migrationBuilder.CreateTable(
                name: "assigned_user_id",
                schema: "ss14",
                columns: table => new
                {
                    assigned_user_id_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assigned_user_id", x => x.assigned_user_id_id);
                });

            migrationBuilder.CreateTable(
                name: "ban_template",
                schema: "ss14",
                columns: table => new
                {
                    ban_template_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    length = table.Column<TimeSpan>(type: "interval", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    exempt_flags = table.Column<int>(type: "integer", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    auto_delete = table.Column<bool>(type: "boolean", nullable: false),
                    hidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ban_template", x => x.ban_template_id);
                });

            migrationBuilder.CreateTable(
                name: "blacklist",
                schema: "ss14",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blacklist", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "ipintel_cache",
                schema: "ss14",
                columns: table => new
                {
                    ipintel_cache_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address = table.Column<IPAddress>(type: "inet", nullable: false),
                    time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    score = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ipintel_cache", x => x.ipintel_cache_id);
                });

            migrationBuilder.CreateTable(
                name: "play_time",
                schema: "ss14",
                columns: table => new
                {
                    play_time_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tracker = table.Column<string>(type: "text", nullable: false),
                    time_spent = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_play_time", x => x.play_time_id);
                });

            migrationBuilder.CreateTable(
                name: "player",
                schema: "ss14",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_seen_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_seen_user_name = table.Column<string>(type: "text", nullable: false),
                    last_seen_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_seen_address = table.Column<IPAddress>(type: "inet", nullable: false),
                    last_seen_hwid = table.Column<byte[]>(type: "bytea", nullable: true),
                    last_seen_hwid_type = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    last_read_rules = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.player_id);
                    table.UniqueConstraint("ak_player_user_id", x => x.user_id);
                    table.CheckConstraint("LastSeenAddressNotIPv6MappedIPv4", "NOT inet '::ffff:0.0.0.0/96' >>= last_seen_address");
                });

            migrationBuilder.CreateTable(
                name: "preference",
                schema: "ss14",
                columns: table => new
                {
                    preference_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    selected_character_slot = table.Column<int>(type: "integer", nullable: false),
                    admin_ooc_color = table.Column<string>(type: "text", nullable: false),
                    construction_favorites = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preference", x => x.preference_id);
                });

            migrationBuilder.CreateTable(
                name: "server",
                schema: "ss14",
                columns: table => new
                {
                    server_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server", x => x.server_id);
                });

            migrationBuilder.CreateTable(
                name: "server_ban_exemption",
                schema: "ss14",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flags = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_ban_exemption", x => x.user_id);
                    table.CheckConstraint("FlagsNotZero", "flags != 0");
                });

            migrationBuilder.CreateTable(
                name: "uploaded_resource_log",
                schema: "ss14",
                columns: table => new
                {
                    uploaded_resource_log_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uploaded_resource_log", x => x.uploaded_resource_log_id);
                });

            migrationBuilder.CreateTable(
                name: "whitelist",
                schema: "ss14",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_whitelist", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "admin",
                schema: "ss14",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    deadminned = table.Column<bool>(type: "boolean", nullable: false),
                    suspended = table.Column<bool>(type: "boolean", nullable: false),
                    admin_rank_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_admin_admin_rank_admin_rank_id",
                        column: x => x.admin_rank_id,
                        principalSchema: "ss14",
                        principalTable: "admin_rank",
                        principalColumn: "admin_rank_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "admin_rank_flag",
                schema: "ss14",
                columns: table => new
                {
                    admin_rank_flag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    flag = table.Column<string>(type: "text", nullable: false),
                    admin_rank_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_rank_flag", x => x.admin_rank_flag_id);
                    table.ForeignKey(
                        name: "FK_admin_rank_flag_admin_rank_admin_rank_id",
                        column: x => x.admin_rank_id,
                        principalSchema: "ss14",
                        principalTable: "admin_rank",
                        principalColumn: "admin_rank_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_whitelists",
                schema: "ss14",
                columns: table => new
                {
                    player_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_whitelists", x => new { x.player_user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_role_whitelists_player_player_user_id",
                        column: x => x.player_user_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile",
                schema: "ss14",
                columns: table => new
                {
                    profile_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    slot = table.Column<int>(type: "integer", nullable: false),
                    char_name = table.Column<string>(type: "text", nullable: false),
                    flavor_text = table.Column<string>(type: "text", nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    sex = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: false),
                    species = table.Column<string>(type: "text", nullable: false),
                    markings = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    hair_name = table.Column<string>(type: "text", nullable: false),
                    hair_color = table.Column<string>(type: "text", nullable: false),
                    facial_hair_name = table.Column<string>(type: "text", nullable: false),
                    facial_hair_color = table.Column<string>(type: "text", nullable: false),
                    eye_color = table.Column<string>(type: "text", nullable: false),
                    skin_color = table.Column<string>(type: "text", nullable: false),
                    spawn_priority = table.Column<int>(type: "integer", nullable: false),
                    pref_unavailable = table.Column<int>(type: "integer", nullable: false),
                    preference_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile", x => x.profile_id);
                    table.ForeignKey(
                        name: "FK_profile_preference_preference_id",
                        column: x => x.preference_id,
                        principalSchema: "ss14",
                        principalTable: "preference",
                        principalColumn: "preference_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "connection_log",
                schema: "ss14",
                columns: table => new
                {
                    connection_log_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    address = table.Column<IPAddress>(type: "inet", nullable: false),
                    hwid = table.Column<byte[]>(type: "bytea", nullable: true),
                    hwid_type = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    denied = table.Column<byte>(type: "smallint", nullable: true),
                    server_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    trust = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connection_log", x => x.connection_log_id);
                    table.CheckConstraint("AddressNotIPv6MappedIPv4", "NOT inet '::ffff:0.0.0.0/96' >>= address");
                    table.ForeignKey(
                        name: "FK_connection_log_server_server_id",
                        column: x => x.server_id,
                        principalSchema: "ss14",
                        principalTable: "server",
                        principalColumn: "server_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "round",
                schema: "ss14",
                columns: table => new
                {
                    round_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    server_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_round", x => x.round_id);
                    table.ForeignKey(
                        name: "FK_round_server_server_id",
                        column: x => x.server_id,
                        principalSchema: "ss14",
                        principalTable: "server",
                        principalColumn: "server_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_flag",
                schema: "ss14",
                columns: table => new
                {
                    admin_flag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    flag = table.Column<string>(type: "text", nullable: false),
                    negative = table.Column<bool>(type: "boolean", nullable: false),
                    admin_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_flag", x => x.admin_flag_id);
                    table.ForeignKey(
                        name: "FK_admin_flag_admin_admin_id",
                        column: x => x.admin_id,
                        principalSchema: "ss14",
                        principalTable: "admin",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "antag",
                schema: "ss14",
                columns: table => new
                {
                    antag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    antag_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_antag", x => x.antag_id);
                    table.ForeignKey(
                        name: "FK_antag_profile_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "ss14",
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job",
                schema: "ss14",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    job_name = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job", x => x.job_id);
                    table.ForeignKey(
                        name: "FK_job_profile_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "ss14",
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile_role_loadout",
                schema: "ss14",
                columns: table => new
                {
                    profile_role_loadout_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    role_name = table.Column<string>(type: "text", nullable: false),
                    entity_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_role_loadout", x => x.profile_role_loadout_id);
                    table.ForeignKey(
                        name: "FK_profile_role_loadout_profile_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "ss14",
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trait",
                schema: "ss14",
                columns: table => new
                {
                    trait_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    trait_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trait", x => x.trait_id);
                    table.ForeignKey(
                        name: "FK_trait_profile_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "ss14",
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_log",
                schema: "ss14",
                columns: table => new
                {
                    round_id = table.Column<int>(type: "integer", nullable: false),
                    admin_log_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    impact = table.Column<short>(type: "smallint", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    json = table.Column<JsonDocument>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_log", x => new { x.round_id, x.admin_log_id });
                    table.ForeignKey(
                        name: "FK_admin_log_round_round_id",
                        column: x => x.round_id,
                        principalSchema: "ss14",
                        principalTable: "round",
                        principalColumn: "round_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_messages",
                schema: "ss14",
                columns: table => new
                {
                    admin_messages_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    round_id = table.Column<int>(type: "integer", nullable: true),
                    player_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    playtime_at_note = table.Column<TimeSpan>(type: "interval", nullable: false),
                    message = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_edited_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    seen = table.Column<bool>(type: "boolean", nullable: false),
                    dismissed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_messages", x => x.admin_messages_id);
                    table.CheckConstraint("NotDismissedAndSeen", "NOT dismissed OR seen");
                    table.ForeignKey(
                        name: "FK_admin_messages_player_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_messages_player_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_messages_player_last_edited_by_id",
                        column: x => x.last_edited_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_messages_player_player_user_id",
                        column: x => x.player_user_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admin_messages_round_round_id",
                        column: x => x.round_id,
                        principalSchema: "ss14",
                        principalTable: "round",
                        principalColumn: "round_id");
                });

            migrationBuilder.CreateTable(
                name: "admin_notes",
                schema: "ss14",
                columns: table => new
                {
                    admin_notes_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    round_id = table.Column<int>(type: "integer", nullable: true),
                    player_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    playtime_at_note = table.Column<TimeSpan>(type: "interval", nullable: false),
                    message = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_edited_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    secret = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_notes", x => x.admin_notes_id);
                    table.ForeignKey(
                        name: "FK_admin_notes_player_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_notes_player_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_notes_player_last_edited_by_id",
                        column: x => x.last_edited_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_notes_player_player_user_id",
                        column: x => x.player_user_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admin_notes_round_round_id",
                        column: x => x.round_id,
                        principalSchema: "ss14",
                        principalTable: "round",
                        principalColumn: "round_id");
                });

            migrationBuilder.CreateTable(
                name: "admin_watchlists",
                schema: "ss14",
                columns: table => new
                {
                    admin_watchlists_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    round_id = table.Column<int>(type: "integer", nullable: true),
                    player_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    playtime_at_note = table.Column<TimeSpan>(type: "interval", nullable: false),
                    message = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_edited_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_watchlists", x => x.admin_watchlists_id);
                    table.ForeignKey(
                        name: "FK_admin_watchlists_player_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_watchlists_player_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_watchlists_player_last_edited_by_id",
                        column: x => x.last_edited_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admin_watchlists_player_player_user_id",
                        column: x => x.player_user_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admin_watchlists_round_round_id",
                        column: x => x.round_id,
                        principalSchema: "ss14",
                        principalTable: "round",
                        principalColumn: "round_id");
                });

            migrationBuilder.CreateTable(
                name: "player_round",
                schema: "ss14",
                columns: table => new
                {
                    players_id = table.Column<int>(type: "integer", nullable: false),
                    rounds_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_round", x => new { x.players_id, x.rounds_id });
                    table.ForeignKey(
                        name: "FK_player_round_player_players_id",
                        column: x => x.players_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "player_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_round_round_rounds_id",
                        column: x => x.rounds_id,
                        principalSchema: "ss14",
                        principalTable: "round",
                        principalColumn: "round_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "server_ban",
                schema: "ss14",
                columns: table => new
                {
                    server_ban_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    round_id = table.Column<int>(type: "integer", nullable: true),
                    player_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    playtime_at_note = table.Column<TimeSpan>(type: "interval", nullable: false),
                    address = table.Column<NpgsqlInet>(type: "inet", nullable: true),
                    hwid = table.Column<byte[]>(type: "bytea", nullable: true),
                    hwid_type = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    ban_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    banning_admin = table.Column<Guid>(type: "uuid", nullable: true),
                    last_edited_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    exempt_flags = table.Column<int>(type: "integer", nullable: false),
                    auto_delete = table.Column<bool>(type: "boolean", nullable: false),
                    hidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_ban", x => x.server_ban_id);
                    table.CheckConstraint("AddressNotIPv6MappedIPv4", "NOT inet '::ffff:0.0.0.0/96' >>= address");
                    table.CheckConstraint("HaveEitherAddressOrUserIdOrHWId", "address IS NOT NULL OR player_user_id IS NOT NULL OR hwid IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_server_ban_player_banning_admin",
                        column: x => x.banning_admin,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_server_ban_player_last_edited_by_id",
                        column: x => x.last_edited_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_server_ban_round_round_id",
                        column: x => x.round_id,
                        principalSchema: "ss14",
                        principalTable: "round",
                        principalColumn: "round_id");
                });

            migrationBuilder.CreateTable(
                name: "server_role_ban",
                schema: "ss14",
                columns: table => new
                {
                    server_role_ban_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    round_id = table.Column<int>(type: "integer", nullable: true),
                    player_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    playtime_at_note = table.Column<TimeSpan>(type: "interval", nullable: false),
                    address = table.Column<NpgsqlInet>(type: "inet", nullable: true),
                    hwid = table.Column<byte[]>(type: "bytea", nullable: true),
                    hwid_type = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    ban_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    banning_admin = table.Column<Guid>(type: "uuid", nullable: true),
                    last_edited_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hidden = table.Column<bool>(type: "boolean", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_role_ban", x => x.server_role_ban_id);
                    table.CheckConstraint("AddressNotIPv6MappedIPv4", "NOT inet '::ffff:0.0.0.0/96' >>= address");
                    table.CheckConstraint("HaveEitherAddressOrUserIdOrHWId", "address IS NOT NULL OR player_user_id IS NOT NULL OR hwid IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_server_role_ban_player_banning_admin",
                        column: x => x.banning_admin,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_server_role_ban_player_last_edited_by_id",
                        column: x => x.last_edited_by_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_server_role_ban_round_round_id",
                        column: x => x.round_id,
                        principalSchema: "ss14",
                        principalTable: "round",
                        principalColumn: "round_id");
                });

            migrationBuilder.CreateTable(
                name: "profile_loadout_group",
                schema: "ss14",
                columns: table => new
                {
                    profile_loadout_group_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_role_loadout_id = table.Column<int>(type: "integer", nullable: false),
                    group_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_loadout_group", x => x.profile_loadout_group_id);
                    table.ForeignKey(
                        name: "FK_profile_loadout_group_profile_role_loadout_profile_role_loa~",
                        column: x => x.profile_role_loadout_id,
                        principalSchema: "ss14",
                        principalTable: "profile_role_loadout",
                        principalColumn: "profile_role_loadout_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_log_player",
                schema: "ss14",
                columns: table => new
                {
                    round_id = table.Column<int>(type: "integer", nullable: false),
                    log_id = table.Column<int>(type: "integer", nullable: false),
                    player_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_log_player", x => new { x.round_id, x.log_id, x.player_user_id });
                    table.ForeignKey(
                        name: "FK_admin_log_player_admin_log_round_id_log_id",
                        columns: x => new { x.round_id, x.log_id },
                        principalSchema: "ss14",
                        principalTable: "admin_log",
                        principalColumns: new[] { "round_id", "admin_log_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admin_log_player_player_player_user_id",
                        column: x => x.player_user_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "server_ban_hit",
                schema: "ss14",
                columns: table => new
                {
                    server_ban_hit_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ban_id = table.Column<int>(type: "integer", nullable: false),
                    connection_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_ban_hit", x => x.server_ban_hit_id);
                    table.ForeignKey(
                        name: "FK_server_ban_hit_connection_log_connection_id",
                        column: x => x.connection_id,
                        principalSchema: "ss14",
                        principalTable: "connection_log",
                        principalColumn: "connection_log_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_server_ban_hit_server_ban_ban_id",
                        column: x => x.ban_id,
                        principalSchema: "ss14",
                        principalTable: "server_ban",
                        principalColumn: "server_ban_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "server_unban",
                schema: "ss14",
                columns: table => new
                {
                    unban_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ban_id = table.Column<int>(type: "integer", nullable: false),
                    unbanning_admin = table.Column<Guid>(type: "uuid", nullable: true),
                    unban_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_unban", x => x.unban_id);
                    table.ForeignKey(
                        name: "FK_server_unban_server_ban_ban_id",
                        column: x => x.ban_id,
                        principalSchema: "ss14",
                        principalTable: "server_ban",
                        principalColumn: "server_ban_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "server_role_unban",
                schema: "ss14",
                columns: table => new
                {
                    role_unban_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ban_id = table.Column<int>(type: "integer", nullable: false),
                    unbanning_admin = table.Column<Guid>(type: "uuid", nullable: true),
                    unban_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_role_unban", x => x.role_unban_id);
                    table.ForeignKey(
                        name: "FK_server_role_unban_server_role_ban_ban_id",
                        column: x => x.ban_id,
                        principalSchema: "ss14",
                        principalTable: "server_role_ban",
                        principalColumn: "server_role_ban_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile_loadout",
                schema: "ss14",
                columns: table => new
                {
                    profile_loadout_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_loadout_group_id = table.Column<int>(type: "integer", nullable: false),
                    loadout_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_loadout", x => x.profile_loadout_id);
                    table.ForeignKey(
                        name: "FK_profile_loadout_profile_loadout_group_profile_loadout_group~",
                        column: x => x.profile_loadout_group_id,
                        principalSchema: "ss14",
                        principalTable: "profile_loadout_group",
                        principalColumn: "profile_loadout_group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_admin_rank_id",
                schema: "ss14",
                table: "admin",
                column: "admin_rank_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_flag_admin_id",
                schema: "ss14",
                table: "admin_flag",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_flag_flag_admin_id",
                schema: "ss14",
                table: "admin_flag",
                columns: new[] { "flag", "admin_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admin_log_date",
                schema: "ss14",
                table: "admin_log",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_admin_log_message",
                schema: "ss14",
                table: "admin_log",
                column: "message")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_admin_log_type",
                schema: "ss14",
                table: "admin_log",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_admin_log_player_player_user_id",
                schema: "ss14",
                table: "admin_log_player",
                column: "player_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_messages_created_by_id",
                schema: "ss14",
                table: "admin_messages",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_messages_deleted_by_id",
                schema: "ss14",
                table: "admin_messages",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_messages_last_edited_by_id",
                schema: "ss14",
                table: "admin_messages",
                column: "last_edited_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_messages_player_user_id",
                schema: "ss14",
                table: "admin_messages",
                column: "player_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_messages_round_id",
                schema: "ss14",
                table: "admin_messages",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_notes_created_by_id",
                schema: "ss14",
                table: "admin_notes",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_notes_deleted_by_id",
                schema: "ss14",
                table: "admin_notes",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_notes_last_edited_by_id",
                schema: "ss14",
                table: "admin_notes",
                column: "last_edited_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_notes_player_user_id",
                schema: "ss14",
                table: "admin_notes",
                column: "player_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_notes_round_id",
                schema: "ss14",
                table: "admin_notes",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_rank_flag_admin_rank_id",
                schema: "ss14",
                table: "admin_rank_flag",
                column: "admin_rank_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_rank_flag_flag_admin_rank_id",
                schema: "ss14",
                table: "admin_rank_flag",
                columns: new[] { "flag", "admin_rank_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admin_watchlists_created_by_id",
                schema: "ss14",
                table: "admin_watchlists",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_watchlists_deleted_by_id",
                schema: "ss14",
                table: "admin_watchlists",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_watchlists_last_edited_by_id",
                schema: "ss14",
                table: "admin_watchlists",
                column: "last_edited_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_watchlists_player_user_id",
                schema: "ss14",
                table: "admin_watchlists",
                column: "player_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_watchlists_round_id",
                schema: "ss14",
                table: "admin_watchlists",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "IX_antag_profile_id_antag_name",
                schema: "ss14",
                table: "antag",
                columns: new[] { "profile_id", "antag_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_assigned_user_id_user_id",
                schema: "ss14",
                table: "assigned_user_id",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_assigned_user_id_user_name",
                schema: "ss14",
                table: "assigned_user_id",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_connection_log_server_id",
                schema: "ss14",
                table: "connection_log",
                column: "server_id");

            migrationBuilder.CreateIndex(
                name: "IX_connection_log_time",
                schema: "ss14",
                table: "connection_log",
                column: "time");

            migrationBuilder.CreateIndex(
                name: "IX_connection_log_user_id",
                schema: "ss14",
                table: "connection_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_one_high_priority",
                schema: "ss14",
                table: "job",
                column: "profile_id",
                unique: true,
                filter: "priority = 3");

            migrationBuilder.CreateIndex(
                name: "IX_job_profile_id",
                schema: "ss14",
                table: "job",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_profile_id_job_name",
                schema: "ss14",
                table: "job",
                columns: new[] { "profile_id", "job_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_play_time_player_id_tracker",
                schema: "ss14",
                table: "play_time",
                columns: new[] { "player_id", "tracker" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_last_seen_user_name",
                schema: "ss14",
                table: "player",
                column: "last_seen_user_name");

            migrationBuilder.CreateIndex(
                name: "IX_player_user_id",
                schema: "ss14",
                table: "player",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_round_rounds_id",
                schema: "ss14",
                table: "player_round",
                column: "rounds_id");

            migrationBuilder.CreateIndex(
                name: "IX_preference_user_id",
                schema: "ss14",
                table: "preference",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profile_preference_id",
                schema: "ss14",
                table: "profile",
                column: "preference_id");

            migrationBuilder.CreateIndex(
                name: "IX_profile_slot_preference_id",
                schema: "ss14",
                table: "profile",
                columns: new[] { "slot", "preference_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profile_loadout_profile_loadout_group_id",
                schema: "ss14",
                table: "profile_loadout",
                column: "profile_loadout_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_profile_loadout_group_profile_role_loadout_id",
                schema: "ss14",
                table: "profile_loadout_group",
                column: "profile_role_loadout_id");

            migrationBuilder.CreateIndex(
                name: "IX_profile_role_loadout_profile_id",
                schema: "ss14",
                table: "profile_role_loadout",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_round_server_id",
                schema: "ss14",
                table: "round",
                column: "server_id");

            migrationBuilder.CreateIndex(
                name: "IX_round_start_date",
                schema: "ss14",
                table: "round",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "IX_server_ban_address",
                schema: "ss14",
                table: "server_ban",
                column: "address");

            migrationBuilder.CreateIndex(
                name: "IX_server_ban_banning_admin",
                schema: "ss14",
                table: "server_ban",
                column: "banning_admin");

            migrationBuilder.CreateIndex(
                name: "IX_server_ban_last_edited_by_id",
                schema: "ss14",
                table: "server_ban",
                column: "last_edited_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_ban_player_user_id",
                schema: "ss14",
                table: "server_ban",
                column: "player_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_ban_round_id",
                schema: "ss14",
                table: "server_ban",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_ban_hit_ban_id",
                schema: "ss14",
                table: "server_ban_hit",
                column: "ban_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_ban_hit_connection_id",
                schema: "ss14",
                table: "server_ban_hit",
                column: "connection_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_role_ban_address",
                schema: "ss14",
                table: "server_role_ban",
                column: "address");

            migrationBuilder.CreateIndex(
                name: "IX_server_role_ban_banning_admin",
                schema: "ss14",
                table: "server_role_ban",
                column: "banning_admin");

            migrationBuilder.CreateIndex(
                name: "IX_server_role_ban_last_edited_by_id",
                schema: "ss14",
                table: "server_role_ban",
                column: "last_edited_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_role_ban_player_user_id",
                schema: "ss14",
                table: "server_role_ban",
                column: "player_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_role_ban_round_id",
                schema: "ss14",
                table: "server_role_ban",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_role_unban_ban_id",
                schema: "ss14",
                table: "server_role_unban",
                column: "ban_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_server_unban_ban_id",
                schema: "ss14",
                table: "server_unban",
                column: "ban_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trait_profile_id_trait_name",
                schema: "ss14",
                table: "trait",
                columns: new[] { "profile_id", "trait_name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_flag",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "admin_log_player",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "admin_messages",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "admin_notes",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "admin_rank_flag",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "admin_watchlists",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "antag",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "assigned_user_id",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "ban_template",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "blacklist",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "ipintel_cache",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "job",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "play_time",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "player_round",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "profile_loadout",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "role_whitelists",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "server_ban_exemption",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "server_ban_hit",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "server_role_unban",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "server_unban",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "trait",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "uploaded_resource_log",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "whitelist",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "admin",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "admin_log",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "profile_loadout_group",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "connection_log",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "server_role_ban",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "server_ban",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "admin_rank",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "profile_role_loadout",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "player",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "round",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "profile",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "server",
                schema: "ss14");

            migrationBuilder.DropTable(
                name: "preference",
                schema: "ss14");
        }
    }
}
