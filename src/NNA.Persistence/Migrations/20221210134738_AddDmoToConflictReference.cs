using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddDmoToConflictReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NnaMovieCharacterConflicts_Characters_CharacterId",
                table: "NnaMovieCharacterConflicts");

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterId",
                table: "NnaMovieCharacterConflicts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "DmoId",
                table: "NnaMovieCharacterConflicts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_NnaMovieCharacterConflicts_DmoId",
                table: "NnaMovieCharacterConflicts",
                column: "DmoId");

            migrationBuilder.AddForeignKey(
                name: "FK_NnaMovieCharacterConflicts_Characters_CharacterId",
                table: "NnaMovieCharacterConflicts",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NnaMovieCharacterConflicts_Dmos_DmoId",
                table: "NnaMovieCharacterConflicts",
                column: "DmoId",
                principalTable: "Dmos",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NnaMovieCharacterConflicts_Characters_CharacterId",
                table: "NnaMovieCharacterConflicts");

            migrationBuilder.DropForeignKey(
                name: "FK_NnaMovieCharacterConflicts_Dmos_DmoId",
                table: "NnaMovieCharacterConflicts");

            migrationBuilder.DropIndex(
                name: "IX_NnaMovieCharacterConflicts_DmoId",
                table: "NnaMovieCharacterConflicts");

            migrationBuilder.DropColumn(
                name: "DmoId",
                table: "NnaMovieCharacterConflicts");

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterId",
                table: "NnaMovieCharacterConflicts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NnaMovieCharacterConflicts_Characters_CharacterId",
                table: "NnaMovieCharacterConflicts",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
