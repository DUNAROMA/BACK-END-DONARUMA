using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using Npgsql;

namespace DonarumaAPI_Data.Services
{
    public class MetodosPagoService : IMetodosPagoService
    {
        private readonly PostgreSQLConfiguration _connectionConfig;

        public MetodosPagoService(PostgreSQLConfiguration connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        protected NpgsqlConnection dbConnection() => new NpgsqlConnection(_connectionConfig.ConnectionString);

        public async Task<IEnumerable<MetodoPagoDto>> GetMetodosPagoUsuarioAsync(long idUsuario)
        {
            using var db = dbConnection();
            // Dapper mapea automáticamente snake_case de SQL a PascalCase de C# si los nombres coinciden
            var sql = "SELECT numero_tarjeta AS NumeroTarjeta, nombre_titular AS NombreTitular FROM buscar_metodos_pago_usuario(@id)";
            return await db.QueryAsync<MetodoPagoDto>(sql, new { id = idUsuario });
        }

        public async Task<string> EliminarMetodoPagoAsync(long idUsuario, long idTarjeta)
        {
            using var db = dbConnection();
            var sql = "SELECT eliminar_metodo_pago_usuario(@u, @t)";
            var resultado = await db.ExecuteScalarAsync<string>(sql, new { u = idUsuario, t = idTarjeta });
            return resultado ?? "Operación completada";
        }
    }
}