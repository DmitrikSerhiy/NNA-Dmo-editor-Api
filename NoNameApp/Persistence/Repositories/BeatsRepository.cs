using Dapper;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Entities;
using Model.Enums;
using Serilog;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    // ReSharper disable once UnusedMember.Global
    public class BeatsRepository : IBeatsRepository {
        private readonly string _dapperConnectionString;
        private DbProviderFactory Factory => MySql.Data.MySqlClient.MySqlClientFactory.Instance;

        public BeatsRepository(IConfiguration configuration) {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            _dapperConnectionString = configuration.GetConnectionString("ConnectionForDapper");
        }

        public async Task<Dmo> CreateDmoAsync(Dmo dmoFromClient, Guid userId) {
            try {
                await using var db = GetMySqlConnection();
                var dmoWithIdentity = new Dmo();
                var createResult = await db.ExecuteAsync(
                    "INSERT INTO dmos (Id, DateOfCreation, Name, MovieTitle, DmoStatus, ShortComment, Mark, NoNameUserId) " +
                    $"VALUES('{dmoWithIdentity.Id}', {dmoWithIdentity.DateOfCreation}, '{dmoFromClient.Name}', '{dmoFromClient.MovieTitle}', " +
                    $"{(short)DmoStatus.New}, '{dmoFromClient.ShortComment}', {dmoFromClient.Mark}, '{userId}')");
                if (createResult != 1) {
                    return null;
                }

                return await db.QueryFirstAsync<Dmo>(
                    $"SELECT Id, Name, MovieTitle, ShortComment FROM dmos WHERE Id = '{dmoWithIdentity.Id}' and NoNameUserId = '{userId}'");
            } catch (Exception ex) {
                Log.Error("Error while creating new dmo", ex);
                return null;
            }
        }

        public async Task<Dmo> EditDmoAsync(Dmo dmoFromClient, Guid userId) {
            try {
                await using var db = GetMySqlConnection();
                var editResult = await db.ExecuteAsync(
                    $"UPDATE dmos set Name = '{dmoFromClient.Name}', MovieTitle = '{dmoFromClient.MovieTitle}', ShortComment = '{dmoFromClient.ShortComment}' " +
                    $"WHERE Id = '{dmoFromClient.Id}' and NoNameUserId = '{userId}'");
                if (editResult != 1) {
                    return null;
                }
                return await db.QueryFirstAsync<Dmo>(
                    $"SELECT Id, Name, MovieTitle, ShortComment FROM dmos WHERE Id = '{dmoFromClient.Id}' and NoNameUserId = '{userId}'");
            }
            catch (Exception ex) {
                Log.Error("Error while editing dmo info", ex);
                return null;
            }
        }

        public async Task<Dmo> LoadDmoAsync(Guid dmoId, Guid userId) {
            try {
                await using var db = GetMySqlConnection();
                return await db.QueryFirstOrDefaultAsync<Dmo>(
                    $"SELECT Id, Name, MovieTitle, DmoStatus, ShortComment, Mark, BeatsJson FROM dmos WHERE Id = '{dmoId}' and NoNameUserId = '{userId}'");
            } catch (Exception ex) {
                Log.Error("Error while getting dmo", ex);
                return null;
            }
        }

        public async Task<BeatUpdateStatus> UpdateBeatsAsync(string jsonBeats, Guid dmoId) {
            try {
                await using var db = GetMySqlConnection();
                var result = await db.ExecuteAsync($"update dmos set BeatsJson = '{jsonBeats}' where Id = '{dmoId}'");
                if (result != 1) {
                    return BeatUpdateStatus.SqlUpdateInvalid;
                }
            } catch (Exception ex) {
                Log.Error("Error while updating beats json", ex);
                return BeatUpdateStatus.Failed;
            }

            return BeatUpdateStatus.Success;
        }


        // ReSharper disable PossibleNullReferenceException
        private DbConnection GetMySqlConnection(bool open = true,
            bool convertZeroDatetime = false, bool allowZeroDatetime = false) {
            var csb = Factory.CreateConnectionStringBuilder();
            csb.ConnectionString = _dapperConnectionString;
            ((dynamic)csb).AllowZeroDateTime = allowZeroDatetime;
            ((dynamic)csb).ConvertZeroDateTime = convertZeroDatetime;
            var connection = Factory.CreateConnection();
            connection.ConnectionString = csb.ConnectionString;
            if (open) connection.Open();
            return connection;
        }
    }
}
