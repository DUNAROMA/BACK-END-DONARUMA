using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DonarumaAPI_DTOs.Compras;
using Npgsql;
using DonarumaAPI_Data.Data;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComprasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarCompra([FromBody] ProcesarCompraDto request)
        {
            try
            {
                // Parámetros para la función de PostgreSQL
                var pIdUsuario = new NpgsqlParameter("p_idusuario", request.IdUsuario);
                var pDireccion = new NpgsqlParameter("p_direccion", request.Direccion ?? (object)DBNull.Value);
                var pIdTarjeta = new NpgsqlParameter("p_idtarjeta", request.IdTarjeta);
                var pPerfumes = new NpgsqlParameter("p_perfumes_ids", request.PerfumesIds);
                var pCantidades = new NpgsqlParameter("p_cantidades", request.Cantidades);

                // Ejecutamos la función. SQLQueryRaw<string> porque ahora devuelve TEXT
                var resultado = await _context.Database
                    .SqlQueryRaw<string>(
                        "SELECT procesar_compra_con_stock(@p_idusuario, @p_direccion, @p_idtarjeta, @p_perfumes_ids, @p_cantidades)",
                        pIdUsuario, pDireccion, pIdTarjeta, pPerfumes, pCantidades
                    )
                    .ToListAsync();

                var mensaje = resultado.FirstOrDefault();

                return Ok(new { mensaje });
            }
            catch (PostgresException ex)
            {
                // Aquí atrapamos los RAISE EXCEPTION de la función (ej: "Stock insuficiente" o "Seguridad")
                return BadRequest(new { error = "Error en la base de datos", detalle = ex.MessageText });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno", detalle = ex.Message });
            }
        }
    }
}