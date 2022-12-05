using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddConflictOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NnaDmoConflicts");

            migrationBuilder.CreateTable(
                name: "NnaMovieCharacterConflicts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PairOrder = table.Column<int>(type: "int", nullable: false),
                    CharacterType = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    Achieved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfCreation = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NnaMovieCharacterConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NnaMovieCharacterConflicts_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NnaMovieCharacterConflicts_CharacterId",
                table: "NnaMovieCharacterConflicts",
                column: "CharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NnaMovieCharacterConflicts");

            migrationBuilder.CreateTable(
                name: "NnaDmoConflicts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Achieved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CharacterType = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    DateOfCreation = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NnaDmoConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NnaDmoConflicts_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NnaDmoConflicts_CharacterId",
                table: "NnaDmoConflicts",
                column: "CharacterId");
        }
    }
}
