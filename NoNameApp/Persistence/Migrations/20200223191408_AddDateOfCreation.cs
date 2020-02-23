using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddDateOfCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DmoUserDmoCollection_Dmos_DmoId",
                table: "DmoUserDmoCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_DmoUserDmoCollection_UserDmoCollections_UserDmoCollectionId",
                table: "DmoUserDmoCollection");

            migrationBuilder.AddColumn<long>(
                name: "DateOfCreation",
                table: "UserDmoCollections",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "DateOfCreation",
                table: "Dmos",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_DmoUserDmoCollection_Dmos_DmoId",
                table: "DmoUserDmoCollection",
                column: "DmoId",
                principalTable: "Dmos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DmoUserDmoCollection_UserDmoCollections_UserDmoCollectionId",
                table: "DmoUserDmoCollection",
                column: "UserDmoCollectionId",
                principalTable: "UserDmoCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DmoUserDmoCollection_Dmos_DmoId",
                table: "DmoUserDmoCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_DmoUserDmoCollection_UserDmoCollections_UserDmoCollectionId",
                table: "DmoUserDmoCollection");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "UserDmoCollections");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "Dmos");

            migrationBuilder.AddForeignKey(
                name: "FK_DmoUserDmoCollection_Dmos_DmoId",
                table: "DmoUserDmoCollection",
                column: "DmoId",
                principalTable: "Dmos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DmoUserDmoCollection_UserDmoCollections_UserDmoCollectionId",
                table: "DmoUserDmoCollection",
                column: "UserDmoCollectionId",
                principalTable: "UserDmoCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
