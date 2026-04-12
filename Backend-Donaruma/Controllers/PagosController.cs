// Controllers/PagosController.cs
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PagosController : ControllerBase
    {
        private readonly IPagosService _pagosService;

        public PagosController(IPagosService pagosService)
        {
            _pagosService = pagosService;
        }

        [HttpPost("crear-sesion")]
        public async Task<IActionResult> CrearSesion([FromBody] CrearSesionDto dto)
        {
            try
            {
                var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (idUsuarioClaim == null)
                    return Unauthorized("No se pudo identificar al usuario.");

                var (sessionId, url) = await _pagosService.CrearSesionAsync(long.Parse(idUsuarioClaim), dto);

                return Ok(new { id = sessionId, url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("confirmar")]
        [AllowAnonymous] // No requiere JWT porque viene de la redirección de Stripe
        public async Task<IActionResult> ConfirmarPago([FromQuery] string session_id)
        {
            try
            {
                var resultado = await _pagosService.ConfirmarPagoAsync(session_id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}