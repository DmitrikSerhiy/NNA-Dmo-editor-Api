using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddCustomManyToManyBeatToCharacterRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterInBeats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    BeatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfCreation = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInBeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterInBeats_Beats_BeatId",
                        column: x => x.BeatId,
                        principalTable: "Beats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterInBeats_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInBeats_BeatId",
                table: "CharacterInBeats",
                column: "BeatId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInBeats_CharacterId",
                table: "CharacterInBeats",
                column: "CharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterInBeats");
        }
    }
}
