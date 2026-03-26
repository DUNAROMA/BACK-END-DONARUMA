using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using Npgsql;

namespace DonarumaAPI_Data.Services
{
    public class PerfumesStockService : IPerfumesStockService
    {
        private readonly PostgreSQLConfiguration _connectionConfig;

        public PerfumesStockService(PostgreSQLConfiguration connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        protected NpgsqlConnection dbConnection() => new NpgsqlConnection(_connectionConfig.ConnectionString);

        public async Task<IEnumerable<Perfume>> GetPerfumesDisponiblesAsync()
        {
            using var db = dbConnection();
            // Usamos alias en el SQL para que coincidan exactamente con las propiedades de tu clase Perfume
            var sql = @"SELECT idperfume AS IdPerfume, 
                               nombreperfume AS Nombre, 
                               precio AS Precio, 
                               stock AS Stock 
                        FROM fn_listar_perfumes_disponibles()";

            return await db.QueryAsync<Perfume>(sql);
        }
    }
}