using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_DTOs.Compras;
using DonarumaAPI_Data.Data;
using Microsoft.EntityFrameworkCore; // Donde tengas tus DTOs

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetodosPagoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MetodosPagoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/MetodosPago/buscar/{idusuario}
        [HttpGet("buscar/{idusuario}")]
        public async Task<ActionResult<IEnumerable<MetodoPagoDto>>> GetPagosFuncion(long idusuario)
        {
            // Llamamos a la función que ya probaste y funciona en pgAdmin
            var resultado = await _context.MetodosPago
                .FromSqlRaw("SELECT * FROM buscar_metodos_pago_usuario({0}::BIGINT)", idusuario)
                .ToListAsync();

            if (resultado == null || !resultado.Any())
            {
                return NotFound("No se encontraron métodos de pago registrados para este usuario.");
            }

            // Mapeamos a nuestro DTO para la respuesta limpia
            var listaDto = resultado.Select(p => new MetodoPagoDto
            {
                NumeroTarjeta = p.NumeroTarjeta,
                NombreTitular = p.NombreTitular
            }).ToList();

            return Ok(listaDto);
        }

        // DELETE: api/MetodosPago/eliminar/{idusuario}/{idtarjeta}
        [HttpDelete("eliminar/{idusuario}/{idtarjeta}")]
        public async Task<IActionResult> EliminarPagoFuncion(long idusuario, long idtarjeta)
        {
            try
            {
                // Ejecutamos la función y capturamos el mensaje de retorno (TEXT)
                // Usamos interpolación segura de parámetros para evitar inyección SQL
                var resultado = await _context.Database
                    .SqlQueryRaw<string>("SELECT eliminar_metodo_pago_usuario({0}::BIGINT, {1}::BIGINT)", idusuario, idtarjeta)
                    .ToListAsync();

                var mensaje = resultado.FirstOrDefault();

                if (string.IsNullOrEmpty(mensaje))
                {
                    return BadRequest("No se pudo procesar la eliminación.");
                }

                return Ok(new { Respuesta = mensaje });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al ejecutar la función: {ex.Message}");
            }
        }
    }
}