using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.Entities;
using Model.Enums;
using Model.Interfaces.Repositories;
using Serilog;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    // ReSharper disable once UnusedMember.Global
    public class EditorRepository : IEditorRepository {
        private readonly string _dapperConnectionString;
        private DbProviderFactory Factory => SqlClientFactory.Instance;

        public EditorRepository(IConfiguration configuration) {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            _dapperConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> CreateDmoAsync(Dmo dmo) {
            await using var db = await OpenAndGetConnection();
            var createResult = await db.ExecuteAsync(
                "INSERT INTO [dbo].[Dmos] ([Id],[DateOfCreation],[Name],[MovieTitle] ,[DmoStatus],[ShortComment],[NnaUserId])" +
                $"VALUES('{dmo.Id}','{dmo.DateOfCreation}', '{dmo.Name}', '{dmo.MovieTitle}', '{(short) DmoStatus.New}', '{dmo.ShortComment}', '{dmo.NnaUserId}')");
            return createResult == 1;
        }

        public async Task<Dmo> EditDmoAsync(Dmo dmo, Guid userId) {
            try {
                await using var db = await OpenAndGetConnection();
                var editResult = await db.ExecuteAsync(
                    $"UPDATE Dmos set Name = '{dmo.Name}', MovieTitle = '{dmo.MovieTitle}', ShortComment = '{dmo.ShortComment}' " +
                    $"WHERE Id = '{dmo.Id}' and NnaUserId = '{userId}'");
                if (editResult != 1) {
                    return null;
                }
                return await db.QueryFirstAsync<Dmo>(
                    $"SELECT Id, Name, MovieTitle, ShortComment FROM Dmos WHERE Id = '{dmo.Id}' and NnaUserId = '{userId}'");
            }
            catch (Exception ex) {
                Log.Error(ex, "Error while editing dmo info");
                return null;
            }
        }

        public async Task<Dmo> LoadShortDmoAsync(Guid dmoId, Guid userId) {
            await using var db = await OpenAndGetConnection();
            return await db.QueryFirstOrDefaultAsync<Dmo>(
                $"SELECT Id, Name, MovieTitle, DmoStatus, ShortComment FROM Dmos WHERE Id = '{dmoId}' and NnaUserId = '{userId}'");
        }

        public async Task<BeatUpdateStatus> UpdateBeatsAsync(string jsonBeats, Guid dmoId) {
            try {
                await using var db = await OpenAndGetConnection();
                var result = await db.ExecuteAsync($"update Dmos set BeatsJson = '{jsonBeats}' where Id = '{dmoId}'");
                if (result != 1) {
                    return BeatUpdateStatus.SqlUpdateInvalid;
                }
            } catch (Exception ex) {
                Log.Error(ex, "Error while updating beats json");
                return BeatUpdateStatus.Failed;
            }

            return BeatUpdateStatus.Success;
        }


        private async Task<DbConnection> OpenAndGetConnection() {
            var builder = Factory.CreateConnectionStringBuilder();
            builder.ConnectionString = _dapperConnectionString;
            var connection = Factory.CreateConnection();
            connection.ConnectionString = builder.ConnectionString;
            await connection.OpenAsync();
            return connection;
        }
    }
}
