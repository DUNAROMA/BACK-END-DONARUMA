using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using DonarumaAPI_DTOs.PerfumesDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Ambos")]
    public class PerfumesController : ControllerBase
    {
        private readonly IPerfumeService _perfumeService;

        public PerfumesController(IPerfumeService perfumeService)
        {
            _perfumeService = perfumeService;
        }

        // GET api/perfumes/todos
        [HttpGet("todos")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerTodos()
        {
            var perfumes = await _perfumeService.ObtenerTodos();
            return Ok(perfumes);
        }

        // GET api/perfumes/noche
        [HttpGet("noche")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerDeNoche()
        {
            var perfumes = await _perfumeService.ObtenerDeNoche();
            return Ok(perfumes);
        }

        // GET api/perfumes/ocasion/casual
        [HttpGet("ocasion/{ocasion}")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerPorOcasion(string ocasion)
        {
            var perfumes = await _perfumeService.ObtenerPorOcasion(ocasion);
            return Ok(perfumes);
        }

        // GET api/perfumes/genero/hombre
        [HttpGet("genero/{genero}")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerPorGenero(string genero)
        {
            var perfumes = await _perfumeService.ObtenerPorGenero(genero);
            return Ok(perfumes);
        }

        // GET api/perfumes/marca/chanel
        [HttpGet("marca/{marca}")]
        public async Task<IActionResult> ObtenerPorMarca(string marca)
        {
            var perfumes = await _perfumeService.ObtenerPorMarcaAsync(marca);
            return Ok(perfumes);
        }

        // GET api/perfumes/buscar/noir
        [HttpGet("buscar/{nombre}")]
        public async Task<IActionResult> BuscarPorNombre(string nombre)
        {
            var perfumes = await _perfumeService.BuscarPorNombreAsync(nombre);
            return Ok(perfumes);
        }

        // GET api/perfumes/precio-mayor
        [HttpGet("precio-mayor")]
        public async Task<IActionResult> ObtenerPorPrecioMayor()
        {
            var perfumes = await _perfumeService.ObtenerPorPrecioMayorAsync();
            return Ok(perfumes);
        }

        // GET api/perfumes/precio-menor
        [HttpGet("precio-menor")]
        public async Task<IActionResult> ObtenerPorPrecioMenor()
        {
            var perfumes = await _perfumeService.ObtenerPorPrecioMenorAsync();
            return Ok(perfumes);
        }

        // POST api/perfumes/crear
        [HttpPost("crear")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CrearPerfume([FromBody] PerfumeDTO perfume)
        {
            try
            {
                var nuevoId = await _perfumeService.CrearPerfume(perfume);
                return Ok(new { mensaje = "¡Perfume creado con éxito!", id = nuevoId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear el perfume: " + ex.Message);
            }
        }

        // PUT api/perfumes/actualizar/5
        [HttpPut("actualizar/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ActualizarPerfume(int id, [FromBody] PerfumeDTO perfume)
        {
            try
            {
                if (id != perfume.IdPerfume)
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el del perfume." });

                var fueEditado = await _perfumeService.ActualizarPerfume(perfume);

                if (fueEditado)
                    return Ok(new { mensaje = "¡Perfume actualizado con éxito!" });

                return NotFound(new { mensaje = "No se encontró el perfume para actualizar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar el perfume: " + ex.Message);
            }
        }

        // DELETE api/perfumes/eliminar/5
        [HttpDelete("eliminar/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EliminarPerfume(int id)
        {
            try
            {
                var fueBorrado = await _perfumeService.EliminarPerfume(id);

                if (fueBorrado)
                    return Ok(new { mensaje = "¡Perfume eliminado permanentemente!" });

                return NotFound(new { mensaje = "No se encontró el perfume para eliminar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al eliminar el perfume: " + ex.Message);
            }
        }
    }
}