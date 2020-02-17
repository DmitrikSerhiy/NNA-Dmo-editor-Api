using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class RemoveDmoCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dmos_UserDmoCollections_UserDmoCollectionId",
                table: "Dmos");

            migrationBuilder.DropIndex(
                name: "IX_Dmos_UserDmoCollectionId",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "DmoCount",
                table: "UserDmoCollections");

            migrationBuilder.DropColumn(
                name: "UserDmoCollectionId",
                table: "Dmos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "DmoCount",
                table: "UserDmoCollections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserDmoCollectionId",
                table: "Dmos",
                type: "char(36)",
                nullable: false,
                defaultValue: "");

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
    }
}
