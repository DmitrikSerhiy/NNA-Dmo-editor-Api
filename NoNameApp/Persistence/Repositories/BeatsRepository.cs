using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Entities;

namespace Persistence.Repositories {
    public class BeatsRepository : IBeatsRepository {
        private readonly string _connectionString;
        public BeatsRepository(IConfiguration configuration) {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void UpdateBeats(Beat[] beats) {
            using (IDbConnection db = new SqlConnection(_connectionString)) {



            }

        }
    }
}
