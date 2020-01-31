using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddDmoLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "UserDmoCollections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CollectionName = table.Column<string>(nullable: true),
                    NoNameUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDmoCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDmoCollections_AspNetUsers_NoNameUserId",
                        column: x => x.NoNameUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DmoUserDmoCollections",
                columns: table => new
                {
                    DmoId = table.Column<Guid>(nullable: false),
                    UserDmoCollectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DmoUserDmoCollections", x => new { x.DmoId, x.UserDmoCollectionId });
                    table.ForeignKey(
                        name: "FK_DmoUserDmoCollections_Dmos_DmoId",
                        column: x => x.DmoId,
                        principalTable: "Dmos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DmoUserDmoCollections_UserDmoCollections_UserDmoCollectionId",
                        column: x => x.UserDmoCollectionId,
                        principalTable: "UserDmoCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DmoUserDmoCollections_UserDmoCollectionId",
                table: "DmoUserDmoCollections",
                column: "UserDmoCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDmoCollections_NoNameUserId",
                table: "UserDmoCollections",
                column: "NoNameUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DmoUserDmoCollections");

            migrationBuilder.DropTable(
                name: "UserDmoCollections");

            migrationBuilder.AddColumn<string>(
                name: "NoNameUserId",
                table: "Dmos",
                type: "char(36)",
                nullable: false,
                defaultValue: "");

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
    }
}
