using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.PerfumesDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesController : ControllerBase
    {
        private readonly IPerfumeService _perfumeService;

        // Conectamos el controlador con el servicio de Alan
        public PerfumesController(IPerfumeService perfumeService)
        {
            _perfumeService = perfumeService;
        }

        // Ruta: GET api/perfumes/todos
        [HttpGet("todos")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerTodos()
        {
            var perfumes = await _perfumeService.ObtenerTodos();
            return Ok(perfumes);
        }

        // Ruta: GET api/perfumes/noche
        [HttpGet("noche")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerDeNoche()
        {
            var perfumes = await _perfumeService.ObtenerDeNoche();
            return Ok(perfumes);
        }

        // Ruta: GET api/perfumes/ocasion/casual
        [HttpGet("ocasion/{ocasion}")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerPorOcasion(string ocasion)
        {
            var perfumes = await _perfumeService.ObtenerPorOcasion(ocasion);
            return Ok(perfumes);
        }

        // Ruta: GET api/perfumes/genero/hombre
        [HttpGet("genero/{genero}")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerPorGenero(string genero)
        {
            var perfumes = await _perfumeService.ObtenerPorGenero(genero);
            return Ok(perfumes);
        }

        [HttpPost("crear")]
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
        [HttpGet("marca/{marca}")]
        public async Task<IActionResult> ObtenerPorMarca(string marca)
        {
            var perfumes = await _perfumeService.ObtenerPorMarcaAsync(marca);
            return Ok(perfumes);
        }

        [HttpGet("buscar/{nombre}")]
        public async Task<IActionResult> BuscarPorNombre(string nombre)
        {
            var perfumes = await _perfumeService.BuscarPorNombreAsync(nombre);
            return Ok(perfumes);
        }

        [HttpGet("precio-mayor")]
        public async Task<IActionResult> ObtenerPorPrecioMayor()
        {
            var perfumes = await _perfumeService.ObtenerPorPrecioMayorAsync();
            return Ok(perfumes);
        }

        [HttpGet("precio-menor")]
        public async Task<IActionResult> ObtenerPorPrecioMenor()
        {
            var perfumes = await _perfumeService.ObtenerPorPrecioMenorAsync();
            return Ok(perfumes);
        }
    }
}