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
    internal sealed class EditorRepository : IEditorRepository {
        private readonly string _dapperConnectionString;
        private DbProviderFactory Factory => SqlClientFactory.Instance;


        #region sqlScripts

        private const string CreateDmoScript = 
            "INSERT INTO [dbo].[Dmos] ([Id],[DateOfCreation],[Name],[MovieTitle],[DmoStatus],[ShortComment],[NnaUserId],[HasBeats]) " +
            "VALUES(@id, @dateOfCreation, @name, @movieTitle, @dmoStatus, @shortComment, @nnaUserId, @hasBeats)";

        private const string LoadShortDmoScript =
            "SELECT Id, Name, MovieTitle, ShortComment, DmoStatus, HasBeats FROM Dmos WHERE Id = @id and NnaUserId = @nnaUserId";

        private const string LoadDmoScript =
            "SELECT * FROM Dmos WHERE Id = @id and NnaUserId = @nnaUserId";

        private const string UpdateShortDmoScript =
            "UPDATE Dmos set Name = @name, MovieTitle = @movieTitle, ShortComment = @shortComment, DmoStatus = @dmoStatus WHERE Id = @id and NnaUserId = @nnaUserId";

        private const string UpdateBeatsJsonScript =
            "UPDATE Dmos set BeatsJson = @jsonBeats, HasBeats = @HasBeats where Id = @id and NnaUserId = @nnaUserId";

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
                dmoStatus = (short) DmoStatus.InProgress,
                shortComment = string.IsNullOrWhiteSpace(dmo.ShortComment) ? null : dmo.ShortComment,
                nnaUserId = dmo.NnaUserId,
                hasBeats = false
            });
            return result >= 1;
        }

        public async Task<Dmo> LoadShortDmoAsync(Guid id, Guid nnaUserId) {
            await using var db = await OpenAndGetConnection();
            return await db.QueryFirstOrDefaultAsync<Dmo>(LoadShortDmoScript, new { id, nnaUserId });
        }

        public async Task<Dmo> LoadDmoAsync(Guid id, Guid nnaUserId) {
            await using var db = await OpenAndGetConnection();
            return await db.QueryFirstOrDefaultAsync<Dmo>(LoadDmoScript, new { id, nnaUserId });
        }

        public async Task<bool> UpdateShortDmoAsync(Dmo dmo) {
            await using var db = await OpenAndGetConnection();
            var result = await db.ExecuteAsync(UpdateShortDmoScript, new {
                name = dmo.Name,
                movieTitle = dmo.MovieTitle,
                shortComment = dmo.ShortComment,
                id = dmo.Id,
                nnaUserId = dmo.NnaUserId,
                dmoStatus = dmo.DmoStatus
            });

            return result >= 1;
        }

        public async Task<bool> UpdateJsonBeatsAsync(string jsonBeats, Guid id, Guid nnaUserId) {
            await using var db = await OpenAndGetConnection();
            var result = await db.ExecuteAsync(UpdateBeatsJsonScript, new {jsonBeats, id, nnaUserId, HasBeats = jsonBeats.Length > 0});
            return result >= 1;
        }


        private async Task<SqlConnection> OpenAndGetConnection() {
            var builder = Factory.CreateConnectionStringBuilder();
            // ReSharper disable once PossibleNullReferenceException
            builder.ConnectionString = _dapperConnectionString;
            var connection = Factory.CreateConnection();
            // ReSharper disable once PossibleNullReferenceException
            connection.ConnectionString = builder.ConnectionString;
            await connection.OpenAsync();
            return (SqlConnection)connection;
        }
    }
}
