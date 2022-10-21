using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddCharactersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aliases = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DmoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfCreation = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Dmos_DmoId",
                        column: x => x.DmoId,
                        principalTable: "Dmos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Characters_DmoId",
                table: "Characters",
                column: "DmoId");

            migrationBuilder.CreateIndex(
                name: "IX_NnaCharactersBeats_CharactersId",
                table: "NnaCharactersBeats",
                column: "CharactersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NnaCharactersBeats");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
