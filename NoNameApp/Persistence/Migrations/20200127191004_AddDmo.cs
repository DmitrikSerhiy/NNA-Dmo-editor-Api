using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddDmo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DecompositionNode");

            migrationBuilder.CreateTable(
                name: "Dmos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MovieTitle = table.Column<string>(nullable: true),
                    NoNameUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dmos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dmos_AspNetUsers_NoNameUserId",
                        column: x => x.NoNameUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dmos_NoNameUserId",
                table: "Dmos",
                column: "NoNameUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dmos");

        }
    }
}
