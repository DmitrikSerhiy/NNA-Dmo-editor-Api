using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class RemoveINtermidiateDmoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DmoUserDmoCollections");

            migrationBuilder.CreateTable(
                name: "DmoUserDmoCollection",
                columns: table => new
                {
                    DmoId = table.Column<Guid>(nullable: false),
                    UserDmoCollectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DmoUserDmoCollection", x => new { x.DmoId, x.UserDmoCollectionId });
                    table.ForeignKey(
                        name: "FK_DmoUserDmoCollection_Dmos_DmoId",
                        column: x => x.DmoId,
                        principalTable: "Dmos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DmoUserDmoCollection_UserDmoCollections_UserDmoCollectionId",
                        column: x => x.UserDmoCollectionId,
                        principalTable: "UserDmoCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DmoUserDmoCollection_UserDmoCollectionId",
                table: "DmoUserDmoCollection",
                column: "UserDmoCollectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DmoUserDmoCollection");

            migrationBuilder.CreateTable(
                name: "DmoUserDmoCollections",
                columns: table => new
                {
                    DmoId = table.Column<string>(type: "char(36)", nullable: false),
                    UserDmoCollectionId = table.Column<string>(type: "char(36)", nullable: false)
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
        }
    }
}
