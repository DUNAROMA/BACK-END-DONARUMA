using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_Data.Interfaces;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesController : ControllerBase
    {
        private readonly IPerfumeService _perfumeService;

        public PerfumesController(IPerfumeService perfumeService)
        {
            _perfumeService = perfumeService;
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