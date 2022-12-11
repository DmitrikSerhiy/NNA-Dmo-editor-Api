using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RemovePairOrderFromConflict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PairOrder",
                table: "NnaMovieCharacterConflicts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PairOrder",
                table: "NnaMovieCharacterConflicts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
