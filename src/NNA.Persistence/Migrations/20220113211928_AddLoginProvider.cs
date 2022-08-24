using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddLoginProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[UsersTokens]");
            string script =
                @"CREATE VIEW [dbo].[UsersTokens] WITH SCHEMABINDING AS
                SELECT users.[Id] AS UserId, users.[Email], accessTokens.LoginProvider, accessTokens.TokenKeyId as AccessTokenId, refreshTokens.TokenKeyId as RefreshTokenId
	            FROM [dbo].[AspNetUsers] users 
		            JOIN [dbo].[AspNetUserTokens] accessTokens on users.Id = accessTokens.UserId and accessTokens.Name = 'Access'
		            JOIN [dbo].[AspNetUserTokens] refreshTokens on users.Id = refreshTokens.UserId and refreshTokens.Name = 'Refresh'";
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[UsersTokens]");
            string script =
                @"CREATE VIEW [dbo].[UsersTokens] WITH SCHEMABINDING AS
                SELECT users.[Id] AS UserId, users.[Email], accessTokens.TokenKeyId as AccessTokenId, refreshTokens.TokenKeyId as RefreshTokenId
	            FROM [dbo].[AspNetUsers] users 
		            JOIN [dbo].[AspNetUserTokens] accessTokens on users.Id = accessTokens.UserId and accessTokens.Name = 'Access'
		            JOIN [dbo].[AspNetUserTokens] refreshTokens on users.Id = refreshTokens.UserId and refreshTokens.Name = 'Refresh'";
            migrationBuilder.Sql(script);
        }
    }
}
