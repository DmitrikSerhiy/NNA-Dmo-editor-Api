using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class UpdateDmo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DmoCount",
                table: "UserDmoCollections",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "DmoStatus",
                table: "Dmos",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Mark",
                table: "Dmos",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "ShortComment",
                table: "Dmos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DmoCount",
                table: "UserDmoCollections");

            migrationBuilder.DropColumn(
                name: "DmoStatus",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "Mark",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "ShortComment",
                table: "Dmos");
        }
    }
}
