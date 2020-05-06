using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddBeats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beats",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateOfCreation = table.Column<long>(nullable: false),
                    PlotTimeSpot = table.Column<TimeSpan>(nullable: false),
                    Order = table.Column<short>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DmoId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beats_Dmos_DmoId",
                        column: x => x.DmoId,
                        principalTable: "Dmos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beats_DmoId",
                table: "Beats",
                column: "DmoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beats");
        }
    }
}
