using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RemoveBeatToCharacterRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NnaCharactersBeats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NnaCharactersBeats",
                columns: table => new
                {
                    BeatsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharactersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NnaCharactersBeats", x => new { x.BeatsId, x.CharactersId });
                    table.ForeignKey(
                        name: "FK_NnaCharactersBeats_Beats_BeatsId",
                        column: x => x.BeatsId,
                        principalTable: "Beats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NnaCharactersBeats_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
