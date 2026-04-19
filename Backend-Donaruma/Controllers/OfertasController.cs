using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.OfertasDTOs;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertasController : ControllerBase
    {
        private readonly IOfertasService _ofertasService;

        public OfertasController(IOfertasService ofertasService)
        {
            _ofertasService = ofertasService;
        }

        // listar
        [HttpGet("listar")]
        public async Task<IActionResult> ListarOfertas()
        {
            var ofertas = await _ofertasService.ObtenerOfertasDetalladasAsync();
            return Ok(ofertas);
        }

        // --- ENDPOINTS DEL RELOJ ---
        [HttpGet("reloj")]
        public async Task<IActionResult> ObtenerReloj()
        {
            try
            {
                var reloj = await _ofertasService.ObtenerRelojActivoAsync();
                if (reloj == null) return NotFound(new { message = "El reloj no ha sido configurado." });
                return Ok(reloj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", details = ex.Message });
            }
        }

        [HttpPost("reloj")]
        public async Task<IActionResult> ActualizarReloj([FromBody] RelojDTO relojDto)
        {
            try
            {
                await _ofertasService.ActualizarRelojAsync(relojDto.FechaFinOferta);
                return Ok(new { message = "Reloj actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", details = ex.Message });
            }
        }

        //  ENDPOINTS DEL CATÁLOGO DE OFERTAS 

        [HttpGet("activas")]
        public async Task<IActionResult> ObtenerOfertas()
        {
            try
            {
                var ofertas = await _ofertasService.ObtenerOfertasActivasAsync();
                return Ok(ofertas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", details = ex.Message });
            }
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> GuardarOferta([FromBody] AgregarOfertaDTO ofertaDto)
        {
            try
            {
                var idNuevaOferta = await _ofertasService.AgregarOModificarOfertaAsync(ofertaDto);
                return Ok(new { message = "Oferta guardada exitosamente.", idOferta = idNuevaOferta });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", details = ex.Message });
            }
        }

        [HttpPut("estado/{id}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool activo)
        {
            try
            {
                await _ofertasService.CambiarEstadoOfertaAsync(id, activo);
                return Ok(new { message = activo ? "Oferta publicada." : "Oferta oculta." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", details = ex.Message });
            }
        }
    }
}