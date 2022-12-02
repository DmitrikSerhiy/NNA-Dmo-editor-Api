using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class ExtendCharacter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CharacterContradictsCharacterization",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CharacterContradictsCharacterizationDescription",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Characterization",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Emphathetic",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EmphatheticDescription",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Goal",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Sympathetic",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SympatheticDescription",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnconsciousGoal",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterContradictsCharacterization",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CharacterContradictsCharacterizationDescription",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Characterization",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Emphathetic",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "EmphatheticDescription",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Goal",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Sympathetic",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SympatheticDescription",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "UnconsciousGoal",
                table: "Characters");
        }
    }
}
