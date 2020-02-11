using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddDmoCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserDmoCollectionId",
                table: "Dmos",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Dmos_UserDmoCollectionId",
                table: "Dmos",
                column: "UserDmoCollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dmos_UserDmoCollections_UserDmoCollectionId",
                table: "Dmos",
                column: "UserDmoCollectionId",
                principalTable: "UserDmoCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dmos_UserDmoCollections_UserDmoCollectionId",
                table: "Dmos");

            migrationBuilder.DropIndex(
                name: "IX_Dmos_UserDmoCollectionId",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "UserDmoCollectionId",
                table: "Dmos");
        }
    }
}
