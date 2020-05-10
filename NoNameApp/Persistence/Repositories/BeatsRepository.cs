using Dapper;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Entities;
using Model.Exceptions;
using System;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;

namespace Persistence.Repositories {
    public class BeatsRepository : IBeatsRepository {
        private readonly string _dapperConnectionString;
        private DbProviderFactory Factory => MySql.Data.MySqlClient.MySqlClientFactory.Instance;

        public BeatsRepository(IConfiguration configuration) {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            _dapperConnectionString = configuration.GetConnectionString("ConnectionForDapper");
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
                    throw new UpdateBeatsWSException();
                }
            }
            catch (UpdateBeatsWSException) {
                return BeatUpdateStatus.SqlUpdateFailed;
            }
            catch (Exception ex) {
                Log.Error("Unknown error while updating beats json", ex);
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
