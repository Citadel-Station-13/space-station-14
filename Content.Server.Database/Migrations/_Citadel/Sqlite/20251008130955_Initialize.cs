using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations._Citadel.Sqlite
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "citadel");

            migrationBuilder.CreateTable(
                name: "persisted_world_objects",
                schema: "citadel",
                columns: table => new
                {
                    persisted_world_objects_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    true_name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    notes = table.Column<string>(type: "TEXT", nullable: false),
                    created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    last_modified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    death_count = table.Column<int>(type: "INTEGER", nullable: true),
                    player_id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persisted_world_objects", x => x.persisted_world_objects_id);
                    table.ForeignKey(
                        name: "FK_persisted_world_objects_player_player_id",
                        column: x => x.player_id,
                        principalSchema: "ss14",
                        principalTable: "player",
                        principalColumn: "player_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_persisted_world_objects_player_id",
                schema: "citadel",
                table: "persisted_world_objects",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "persisted_world_objects",
                schema: "citadel");
        }
    }
}
