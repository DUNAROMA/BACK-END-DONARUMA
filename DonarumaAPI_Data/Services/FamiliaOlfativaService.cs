using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.FamiliaOlfativa;
using Npgsql;

namespace DonarumaAPI_Data.Services
{
    public class FamiliaOlfativaService : IFamiliaOlfativaService
    {
        private readonly PostgreSQLConfiguration _connectionConfig;

        public FamiliaOlfativaService(PostgreSQLConfiguration connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        protected NpgsqlConnection dbConnection() => new NpgsqlConnection(_connectionConfig.ConnectionString);

        public async Task<IEnumerable<FamiliaOlfativaDTO>> GetTodosLosPerfumesAsync()
        {
            using var db = dbConnection();
            var sql = "SELECT * FROM obtener_perfumes_completos()";

            // Dapper hace el mapeo automático de nombres de columnas a propiedades del DTO
            return await db.QueryAsync<FamiliaOlfativaDTO>(sql);
        }
    }
}