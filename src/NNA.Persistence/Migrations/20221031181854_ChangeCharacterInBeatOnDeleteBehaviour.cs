using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class ChangeCharacterInBeatOnDeleteBehaviour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterInBeats_Beats_BeatId",
                table: "CharacterInBeats");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterInBeats_Characters_CharacterId",
                table: "CharacterInBeats");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterInBeats_Beats_BeatId",
                table: "CharacterInBeats",
                column: "BeatId",
                principalTable: "Beats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterInBeats_Characters_CharacterId",
                table: "CharacterInBeats",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterInBeats_Beats_BeatId",
                table: "CharacterInBeats");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterInBeats_Characters_CharacterId",
                table: "CharacterInBeats");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterInBeats_Beats_BeatId",
                table: "CharacterInBeats",
                column: "BeatId",
                principalTable: "Beats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterInBeats_Characters_CharacterId",
                table: "CharacterInBeats",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }
    }
}
