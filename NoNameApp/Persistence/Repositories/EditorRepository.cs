using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.Entities;
using Model.Enums;
using Model.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    internal sealed class EditorRepository : IEditorRepository {
        private readonly string _connectionString;

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
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _connectionString = configuration.GetConnectionString("ConnectionForEditor") 
                                ?? throw new ApplicationException("Failed to establish db connection");
        }

        public async Task<bool> CreateDmoAsync(Dmo dmo) {
            var result = await ExecuteAsync(CreateDmoScript, new {
                id = dmo.Id,
                dateOfCreation = dmo.DateOfCreation,
                name = dmo.Name,
                movieTitle = dmo.MovieTitle,
                dmoStatus = (short)DmoStatus.InProgress,
                shortComment = string.IsNullOrWhiteSpace(dmo.ShortComment) ? null : dmo.ShortComment,
                nnaUserId = dmo.NnaUserId,
                hasBeats = false
            });

            return result >= 1;
        }

        public async Task<Dmo> LoadShortDmoAsync(Guid id, Guid nnaUserId) {
            return await QueryAsync<Dmo>(LoadShortDmoScript, new { id, nnaUserId });
        }

        public async Task<Dmo> LoadDmoAsync(Guid id, Guid nnaUserId) {
            return await QueryAsync<Dmo>(LoadDmoScript, new { id, nnaUserId });
        }

        public async Task<bool> UpdateShortDmoAsync(Dmo dmo) {
            var result = await ExecuteAsync(UpdateShortDmoScript, new {
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
            var result = await ExecuteAsync(UpdateBeatsJsonScript, new {
                jsonBeats, 
                id, 
                nnaUserId, 
                HasBeats = jsonBeats.Length > 0
            });
            return result >= 1;
        }


        // Asynchronous Processing=true; <-- that's a part of connection string
        // todo: maybe I should migrate to ADO.NET for real async operations here 
        // because Dapper just does Task.FromResult under the hood. Need to investigate more deeply here
        // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring?view=dotnet-plat-ext-6.0
        // and inspect code of dapper more accurate
        

        private async Task<T> QueryAsync<T>(string request, object parameters) {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection. QueryFirstOrDefaultAsync<T>(request, parameters);
        }

        private async Task<int> ExecuteAsync(string command, object parameters) {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteAsync(command, parameters);
        }
    }
}
