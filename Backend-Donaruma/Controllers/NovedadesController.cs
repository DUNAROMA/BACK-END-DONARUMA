using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.NovedadesDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NovedadesController : ControllerBase
    {
        private readonly INovedadService _novedadService;

        public NovedadesController(INovedadService novedadService)
        {
            _novedadService = novedadService;
        }

        
        [HttpGet("obtener-todas")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var lista = await _novedadService.ObtenerTodas();
            return Ok(lista);
        }

        
        [HttpPost("crear")]
        public async Task<IActionResult> CrearNovedad([FromBody] NovedadesDTO novedad)
        {
            var idGenerado = await _novedadService.CrearNovedad(novedad);

            if (idGenerado > 0)
                return Ok(new { exito = true, mensaje = "Novedad publicada con éxito", idNovedad = idGenerado });

            return BadRequest(new { exito = false, mensaje = "No se pudo crear la publicación" });
        }

        
        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarNovedad([FromBody] NovedadesDTO novedad)
        {
            var exito = await _novedadService.ActualizarNovedad(novedad);

            if (exito)
                return Ok(new { exito = true, mensaje = "Novedad actualizada correctamente" });

            return BadRequest(new { exito = false, mensaje = "No se pudo actualizar la publicación" });
        }

        
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarNovedad(int id)
        {
            var exito = await _novedadService.EliminarNovedad(id);

            if (exito)
                return Ok(new { exito = true, mensaje = "Novedad eliminada" });

            return BadRequest(new { exito = false, mensaje = "No se pudo eliminar la publicación" });
        }
        
        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var novedad = await _novedadService.ObtenerPorId(id);

            if (novedad == null)
                return NotFound(new { exito = false, mensaje = "La publicación no existe" });

            return Ok(novedad);
        }
        
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] NovedadesDTO novedad)
        {
            var exito = await _novedadService.Actualizar(id, novedad);

            if (!exito)
            {
                return NotFound(new { exito = false, mensaje = "No se encontró la publicación a actualizar." });
            }

            return Ok(new { exito = true, mensaje = "Publicación actualizada correctamente." });
        }

    }
}