using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ConsentV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "consent_settings",
                columns: table => new
                {
                    consent_settings_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    profile_id = table.Column<int>(type: "INTEGER", nullable: true),
                    consent_freetext = table.Column<string>(type: "TEXT", nullable: false),
                    consent_freetext_updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consent_settings", x => x.consent_settings_id);
                    table.ForeignKey(
                        name: "FK_consent_settings_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id");
                });

            migrationBuilder.CreateTable(
                name: "consent_freetext_read_receipt",
                columns: table => new
                {
                    consent_freetext_read_receipt_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    read_consent_settings_id = table.Column<int>(type: "INTEGER", nullable: false),
                    reader_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    read_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consent_freetext_read_receipt", x => x.consent_freetext_read_receipt_id);
                    table.ForeignKey(
                        name: "FK_consent_freetext_read_receipt_consent_settings_read_consent_settings_id",
                        column: x => x.read_consent_settings_id,
                        principalTable: "consent_settings",
                        principalColumn: "consent_settings_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consent_toggle",
                columns: table => new
                {
                    consent_toggle_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    consent_settings_id = table.Column<int>(type: "INTEGER", nullable: false),
                    toggle_proto_id = table.Column<string>(type: "TEXT", nullable: false),
                    toggle_proto_state = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consent_toggle", x => x.consent_toggle_id);
                    table.ForeignKey(
                        name: "FK_consent_toggle_consent_settings_consent_settings_id",
                        column: x => x.consent_settings_id,
                        principalTable: "consent_settings",
                        principalColumn: "consent_settings_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_consent_freetext_read_receipt_read_consent_settings_id",
                table: "consent_freetext_read_receipt",
                column: "read_consent_settings_id");

            migrationBuilder.CreateIndex(
                name: "IX_consent_freetext_read_receipt_reader_user_id_read_consent_settings_id",
                table: "consent_freetext_read_receipt",
                columns: new[] { "reader_user_id", "read_consent_settings_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_consent_settings_profile_id",
                table: "consent_settings",
                column: "profile_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_consent_settings_user_id_profile_id",
                table: "consent_settings",
                columns: new[] { "user_id", "profile_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_consent_toggle_consent_settings_id_toggle_proto_id",
                table: "consent_toggle",
                columns: new[] { "consent_settings_id", "toggle_proto_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consent_freetext_read_receipt");

            migrationBuilder.DropTable(
                name: "consent_toggle");

            migrationBuilder.DropTable(
                name: "consent_settings");
        }
    }
}
