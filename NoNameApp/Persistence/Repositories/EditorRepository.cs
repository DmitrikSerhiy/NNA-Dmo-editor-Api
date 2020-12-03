using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.Entities;
using Model.Enums;
using Model.Interfaces.Repositories;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    public class EditorRepository : IEditorRepository {
        private readonly string _dapperConnectionString;
        private DbProviderFactory Factory => SqlClientFactory.Instance;


        #region sqlScripts

        private const string CreateDmoScript = 
            "INSERT INTO [dbo].[Dmos] ([Id],[DateOfCreation],[Name],[MovieTitle] ,[DmoStatus],[ShortComment],[NnaUserId]) " +
            "VALUES(@id, @dateOfCreation, @name, @movieTitle, @dmoStatus, @shortComment, @userId)";

        private const string LoadShortDmoScript =
            "SELECT Id, Name, MovieTitle, ShortComment FROM Dmos WHERE Id = @id and NnaUserId = @userId";

        private const string LoadDmoScript =
            "SELECT * FROM Dmos WHERE Id = @id and NnaUserId = @userId";

        private const string UpdateShortDmoScript =
            "UPDATE Dmos set Name = @name, MovieTitle = @movieTitle, ShortComment = @shortComment WHERE Id = @id and NnaUserId = @userId";

        private const string UpdateBeatsJsonScript =
            "UPDATE Dmos set BeatsJson = @jsonBeats where Id = @id and NnaUserId = @userId";

        #endregion

        public EditorRepository(IConfiguration configuration) {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            _dapperConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> CreateDmoAsync(Dmo dmo) {
            await using var db = await OpenAndGetConnection();
            var result = await db.ExecuteAsync(CreateDmoScript, new {
                id = dmo.Id,
                dateOfCreation = dmo.DateOfCreation,
                name = dmo.Name,
                movieTitle = dmo.MovieTitle,
                dmoStatus = (short) DmoStatus.New,
                shortComment = dmo.ShortComment,
                userId = dmo.NnaUserId
            });
            return result >= 1;
        }

        public async Task<Dmo> LoadShortDmoAsync(Guid id, Guid userId) {
            await using var db = await OpenAndGetConnection();
            return await db.QueryFirstOrDefaultAsync<Dmo>(LoadShortDmoScript, new { id, userId });
        }

        public async Task<Dmo> LoadDmoAsync(Guid id, Guid userId) {
            await using var db = await OpenAndGetConnection();
            return await db.QueryFirstOrDefaultAsync<Dmo>(LoadDmoScript, new { id, userId });
        }

        public async Task<bool> UpdateShortDmoAsync(Dmo dmo) {
            await using var db = await OpenAndGetConnection();
            var result = await db.ExecuteAsync(UpdateShortDmoScript, new {
                name = dmo.Name,
                movieTitle = dmo.MovieTitle,
                shortComment = dmo.ShortComment,
                id = dmo.Id,
                userId = dmo.NnaUserId
            });

            return result >= 1;
        }

        public async Task<bool> UpdateJsonBeatsAsync(string jsonBeats, Guid id, Guid userId) {
            await using var db = await OpenAndGetConnection();
            var result = await db.ExecuteAsync(UpdateBeatsJsonScript, new {jsonBeats, id, userId});
            return result >= 1;
        }


        private async Task<SqlConnection> OpenAndGetConnection() {
            var builder = Factory.CreateConnectionStringBuilder();
            builder.ConnectionString = _dapperConnectionString;
            var connection = Factory.CreateConnection();
            connection.ConnectionString = builder.ConnectionString;
            await connection.OpenAsync();
            return (SqlConnection)connection;
        }
    }
}
