using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.CarritoDTOs;
using Npgsql;

namespace DonarumaAPI_Data.Services
{
    public class CarritoService : ICarritoService
    {
        private readonly PostgreSQLConfiguration _connectionString;

        public CarritoService(PostgreSQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection() => new NpgsqlConnection(_connectionString.ConnectionString);

        public async Task<bool> AgregarAlCarrito(AgregarAlCarritoDTO item)
        {
            var db = dbConnection();
            var sql = @"INSERT INTO ""CarritoItems"" (""IdUsuario"", ""IdPerfume"", ""Cantidad"") 
                        VALUES (@IdUsuario, @IdPerfume, @Cantidad)";

            var result = await db.ExecuteAsync(sql, new { item.IdUsuario, item.IdPerfume, item.Cantidad });
            return result > 0;
        }

        public async Task<IEnumerable<dynamic>> ObtenerCarritoPorUsuario(int idUsuario)
        {
            var db = dbConnection();

            var sql = @"
        SELECT 
            c.""IdCarritoItem"", 
            c.""IdPerfume"", 
            c.""Cantidad"", 
            p.nombreperfume as nombre, 
            p.precio as precio, 
            p.imagen_url as img1
        FROM ""CarritoItems"" c
        INNER JOIN perfumes p ON c.""IdPerfume"" = p.idperfume
        WHERE c.""IdUsuario"" = @IdUsuario";

            return await db.QueryAsync<dynamic>(sql, new { IdUsuario = idUsuario });
        }

        public async Task<bool> EliminarItemCarrito(int idCarritoItem)
        {
            var db = dbConnection();
            var sql = @"DELETE FROM ""CarritoItems"" WHERE ""IdCarritoItem"" = @IdCarritoItem";
            var result = await db.ExecuteAsync(sql, new { IdCarritoItem = idCarritoItem });
            return result > 0;
        }

        public async Task<bool> VaciarCarrito(int idUsuario)
        {
            var db = dbConnection();
            var sql = @"DELETE FROM ""CarritoItems"" WHERE ""IdUsuario"" = @IdUsuario";
            var result = await db.ExecuteAsync(sql, new { IdUsuario = idUsuario });
            return result > 0;
        }

    }
}