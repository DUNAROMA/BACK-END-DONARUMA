// Controllers/ComprasController.cs
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using DonarumaAPI_DTOs.Compras.DonarumaAPI_DTOs.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public ComprasController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarCompra([FromBody] ProcesarCompraDto request)
        {
            try
            {
                var resultado = await _compraService.ProcesarCompraAsync(request);
                return Ok(resultado);
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