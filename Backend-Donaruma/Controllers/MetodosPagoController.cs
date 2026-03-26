using DonarumaAPI_Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetodosPagoController : ControllerBase
    {
        private readonly IMetodosPagoService _pagoService;

        public MetodosPagoController(IMetodosPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpGet("buscar/{idusuario}")]
        public async Task<IActionResult> GetPagos(long idusuario)
        {
            try
            {
                var lista = await _pagoService.GetMetodosPagoUsuarioAsync(idusuario);
                if (!lista.Any()) return NotFound(new { mensaje = "Sin métodos de pago." });

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("eliminar/{idusuario}/{idtarjeta}")]
        public async Task<IActionResult> EliminarPago(long idusuario, long idtarjeta)
        {
            try
            {
                var respuesta = await _pagoService.EliminarMetodoPagoAsync(idusuario, idtarjeta);
                return Ok(new { Respuesta = respuesta });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}