using Dapper;
using Npgsql;
using DonarumaAPI_Data.Interfaces;

namespace DonarumaAPI_Data.Services
{
    public class CompraService : ICompraService
    {
        private readonly PostgreSQLConfiguration _connectionString;

        public CompraService(PostgreSQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected NpgsqlConnection dbConnection() => new NpgsqlConnection(_connectionString.ConnectionString);

        public async Task<bool> ProcesarCompraExitosa(int idUsuario, string sessionId)
        {
            using var db = dbConnection();
            await db.OpenAsync();

            // 🛡️ Iniciamos la Transacción "Todo o Nada"
            using var transaction = await db.BeginTransactionAsync();

            try
            {
                // PASO 1: Leemos qué tenía el usuario en el carrito
                var sqlCarrito = @"SELECT ""IdPerfume"", ""Cantidad"" FROM ""CarritoItems"" WHERE ""IdUsuario"" = @IdUsuario";
                var itemsCarrito = await db.QueryAsync<dynamic>(sqlCarrito, new { IdUsuario = idUsuario });

                if (!itemsCarrito.Any()) return false;

                // PASO 2: Creamos la factura principal
                var sqlCompra = @"
                    INSERT INTO compra (fkidusuario, stripe_session_id) 
                    VALUES (@IdUsuario, @SessionId) 
                    RETURNING idcompra;";

                var idCompra = await db.ExecuteScalarAsync<int>(sqlCompra, new { IdUsuario = idUsuario, SessionId = sessionId }, transaction);

                // PASO 3: Metemos cada perfume a la caja
                var sqlDetalle = @"
                    INSERT INTO detalle_compra (fkidcompra, fkidperfume, cantidad, fecha_compra) 
                    VALUES (@IdCompra, @IdPerfume, @Cantidad, NOW());";

                foreach (var item in itemsCarrito)
                {
                    await db.ExecuteAsync(sqlDetalle, new
                    {
                        IdCompra = idCompra,
                        IdPerfume = item.IdPerfume,
                        Cantidad = item.Cantidad
                    }, transaction);
                }

                // PASO 4: Vaciamos el carrito
                var sqlVaciar = @"DELETE FROM ""CarritoItems"" WHERE ""IdUsuario"" = @IdUsuario";
                await db.ExecuteAsync(sqlVaciar, new { IdUsuario = idUsuario }, transaction);

                // ✅ Si todo sale bien, guardamos definitivamente
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // ❌ Si hay error, deshacemos todo
                await transaction.RollbackAsync();
                System.Console.WriteLine("\n\n❌ Error al guardar la compra: " + ex.Message + "\n\n");
                return false;
            }
        }
    }
}