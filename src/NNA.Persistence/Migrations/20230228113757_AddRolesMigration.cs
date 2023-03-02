using Microsoft.EntityFrameworkCore.Migrations;
using NNA.Domain.Enums;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddRolesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string notActiveUserRole =
                @$" insert into [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
                    VALUES ('{Guid.NewGuid()}', '{Enum.GetName(NnaRoles.NotActiveUser)}', '{Enum.GetName(NnaRoles.NotActiveUser).Normalize().ToUpperInvariant()}', '{Guid.NewGuid()}')";
            
            string activeUserRole =
                @$" insert into [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
                    VALUES ('{Guid.NewGuid()}', '{Enum.GetName(NnaRoles.ActiveUser)}', '{Enum.GetName(NnaRoles.ActiveUser).Normalize().ToUpperInvariant()}', '{Guid.NewGuid()}')";
            
            string superUserRole =
                @$" insert into [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
                   VALUES ('{Guid.NewGuid()}', '{Enum.GetName(NnaRoles.SuperUser)}', '{Enum.GetName(NnaRoles.SuperUser).Normalize().ToUpperInvariant()}', '{Guid.NewGuid()}')";
            
            migrationBuilder.Sql(notActiveUserRole);
            migrationBuilder.Sql(activeUserRole);
            migrationBuilder.Sql(superUserRole);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
