using Dapper;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Entities;
using Model.Exceptions;
using System;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using Model.Enums;
using Serilog;

namespace Persistence.Repositories {
    public class BeatsRepository : IBeatsRepository {
        private readonly string _dapperConnectionString;
        private DbProviderFactory Factory => MySql.Data.MySqlClient.MySqlClientFactory.Instance;

        public BeatsRepository(IConfiguration configuration) {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            _dapperConnectionString = configuration.GetConnectionString("ConnectionForDapper");
        }

        public async Task<UpdateDmoStatus> CreateDmoAsync(Dmo dmoFromClient, Guid userId) {
            try {
                await using var db = GetMySqlConnection();
                var dmoWithIdentity = new Dmo();
                var result = await db.ExecuteAsync(
                    "INSERT INTO dmos (Id, DateOfCreation, Name, MovieTitle, DmoStatus, ShortComment, Mark, NoNameUserId) " +
                    $"VALUES('{dmoWithIdentity.Id}', {dmoWithIdentity.DateOfCreation}, '{dmoFromClient.Name}', '{dmoFromClient.MovieTitle}', " +
                    $"{(Int16)DmoStatus.New}, '{dmoFromClient.ShortComment}', {dmoFromClient.Mark}, '{userId}')");
                if (result != 1) {
                    return UpdateDmoStatus.SqlUpdateInvalid;
                }
            }
            catch (Exception ex) {
                Log.Error("Error when creating new dmo", ex);
                return UpdateDmoStatus.Failed;
            }

            return UpdateDmoStatus.Success;
        }

        public async Task<Dmo> LoadDmoAsync(Guid dmoId, Guid userId) {
            await using var db = GetMySqlConnection();
            return await db.QueryFirstOrDefaultAsync<Dmo>($"select * from dmos where Id = '{dmoId}' and NoNameUserId = '{userId}'");
        }

        public async Task<BeatUpdateStatus> UpdateBeatsAsync(string jsonBeats, Guid dmoId) {
            try {
                await using var db = GetMySqlConnection();
                var result = await db.ExecuteAsync($"update dmos set BeatsJson = '{jsonBeats}' where Id = '{dmoId}'");
                if (result != 1) {
                    return BeatUpdateStatus.SqlUpdateInvalid;
                }
            }
            catch (Exception ex) {
                Log.Error("Error while updating beats json", ex);
                return BeatUpdateStatus.Failed;
            }

            return BeatUpdateStatus.Success;
        }


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
