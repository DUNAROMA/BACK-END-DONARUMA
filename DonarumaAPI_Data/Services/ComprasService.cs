using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using Npgsql;
using System.Data;

namespace DonarumaAPI_Data.Services
{
    public class ComprasService : IComprasService
    {
        private readonly PostgreSQLConfiguration _connectionConfig;

        public ComprasService(PostgreSQLConfiguration connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(_connectionConfig.ConnectionString);
        }

        public async Task<string> ProcesarCompraAsync(ProcesarCompraDto request)
        {
            using var db = dbConnection();
            var sql = @"SELECT public.procesar_compra_con_stock(@p_idusuario, @p_direccion, @p_idtarjeta, @p_perfumes_ids, @p_cantidades)";

            // Dapper maneja automáticamente la apertura de la conexión y los parámetros
            var resultado = await db.ExecuteScalarAsync<string>(sql, new
            {
                p_idusuario = request.IdUsuario,
                p_direccion = request.Direccion,
                p_idtarjeta = request.IdTarjeta,
                p_perfumes_ids = request.PerfumesIds,
                p_cantidades = request.Cantidades
            });

            return resultado;
        }
    }
}