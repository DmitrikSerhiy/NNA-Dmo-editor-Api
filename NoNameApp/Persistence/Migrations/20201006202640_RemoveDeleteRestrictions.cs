using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class RemoveDeleteRestrictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_DmoUserDmoCollection_Dmos_DmoId",
                table: "DmoUserDmoCollection",
                column: "DmoId",
                principalTable: "Dmos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DmoUserDmoCollection_UserDmoCollections_UserDmoCollectionId",
                table: "DmoUserDmoCollection",
                column: "UserDmoCollectionId",
                principalTable: "UserDmoCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DmoUserDmoCollection_Dmos_DmoId",
                table: "DmoUserDmoCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_DmoUserDmoCollection_UserDmoCollections_UserDmoCollectionId",
                table: "DmoUserDmoCollection");
        }
    }
}
