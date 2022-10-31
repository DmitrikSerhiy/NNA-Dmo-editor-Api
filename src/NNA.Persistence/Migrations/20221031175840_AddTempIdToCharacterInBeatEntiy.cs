using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddTempIdToCharacterInBeatEntiy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "CharacterInBeats");

            migrationBuilder.AddColumn<string>(
                name: "TempId",
                table: "CharacterInBeats",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempId",
                table: "CharacterInBeats");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "CharacterInBeats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
