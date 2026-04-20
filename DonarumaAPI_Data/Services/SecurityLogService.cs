using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Model;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Services
{
    public class SecurityLogService : ISecurityLogService
    {
        private readonly string _connectionString;

        public SecurityLogService(PostgreSQLConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<SecurityLog>> ObtenerTodosLosLogs()
        {
            using var db = Connection;

            
            var query = "SELECT * FROM \"SecurityLogs\" ORDER BY \"EventDate\" DESC LIMIT 100;";

            return await db.QueryAsync<SecurityLog>(query);
        }
    }
}