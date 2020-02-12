using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddDmoToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NoNameUserId",
                table: "Dmos",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Dmos_NoNameUserId",
                table: "Dmos",
                column: "NoNameUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dmos_AspNetUsers_NoNameUserId",
                table: "Dmos",
                column: "NoNameUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dmos_AspNetUsers_NoNameUserId",
                table: "Dmos");

            migrationBuilder.DropIndex(
                name: "IX_Dmos_NoNameUserId",
                table: "Dmos");

            migrationBuilder.DropColumn(
                name: "NoNameUserId",
                table: "Dmos");
        }
    }
}
