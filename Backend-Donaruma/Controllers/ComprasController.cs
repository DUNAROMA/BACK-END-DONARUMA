using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_DTOs.Compras;
using DonarumaAPI_Data.Interfaces;
using Npgsql;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly IComprasService _comprasService;

        public ComprasController(IComprasService comprasService)
        {
            _comprasService = comprasService;
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarCompra([FromBody] ProcesarCompraDto request)
        {
            try
            {
                var mensaje = await _comprasService.ProcesarCompraAsync(request);
                return Ok(new { mensaje });
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