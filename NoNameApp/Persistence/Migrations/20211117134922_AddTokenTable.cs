using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddTokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessTokenId",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "RefreshTokenId",
                table: "AspNetUserLogins");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TokenKeyId",
                table: "AspNetUserTokens",
                type: "nvarchar(max)",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserTokens");

            migrationBuilder.DropColumn(
                name: "TokenKeyId",
                table: "AspNetUserTokens");

            migrationBuilder.AddColumn<string>(
                name: "AccessTokenId",
                table: "AspNetUserLogins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshTokenId",
                table: "AspNetUserLogins",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
