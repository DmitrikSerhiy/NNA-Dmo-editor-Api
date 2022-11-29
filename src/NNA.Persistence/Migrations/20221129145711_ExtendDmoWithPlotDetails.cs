using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class ExtendDmoWithPlotDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ControllingIdea",
                table: "Dmos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ControllingIdeaId",
                table: "Dmos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Didacticism",
                table: "Dmos",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DidacticismDescription",
                table: "Dmos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Premise",
                table: "Dmos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControllingIdea",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "ControllingIdeaId",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "Didacticism",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "DidacticismDescription",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "Premise",
                table: "Dmos");
        }
    }
}
