using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_DTOs.Compras;
using Npgsql;
using DonarumaAPI_Data; // Para acceder a PostgreSQLConfiguration

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly PostgreSQLConfiguration _config;

        public ComprasController(PostgreSQLConfiguration config)
        {
            _config = config;
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarCompra([FromBody] ProcesarCompraDto request)
        {
            try
            {
                using var connection = new NpgsqlConnection(_config.ConnectionString);
                await connection.OpenAsync();

                using var command = new NpgsqlCommand("SELECT procesar_compra_con_stock(@p_idusuario, @p_direccion, @p_idtarjeta, @p_perfumes_ids, @p_cantidades)", connection);

                command.Parameters.AddWithValue("p_idusuario", request.IdUsuario);
                command.Parameters.AddWithValue("p_direccion", request.Direccion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("p_idtarjeta", request.IdTarjeta);
                command.Parameters.AddWithValue("p_perfumes_ids", request.PerfumesIds);
                command.Parameters.AddWithValue("p_cantidades", request.Cantidades);

                var mensaje = await command.ExecuteScalarAsync();

                return Ok(new { mensaje = mensaje?.ToString() });
            }
            catch (PostgresException ex)
            {
                return BadRequest(new { error = "Error en BD", detalle = ex.MessageText });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno", detalle = ex.Message });
            }
        }
    }
}