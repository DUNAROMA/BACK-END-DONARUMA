using DonarumaAPI_Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesStockController : ControllerBase
    {
        private readonly IPerfumesStockService _stockService;

        public PerfumesStockController(IPerfumesStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet("disponibles")]
        public async Task<IActionResult> GetPerfumesDisponibles()
        {
            try
            {
                var perfumes = await _stockService.GetPerfumesDisponiblesAsync();

                if (perfumes == null || !perfumes.Any())
                    return NotFound(new { mensaje = "No hay perfumes con stock disponible." });

                return Ok(perfumes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener stock", detalle = ex.Message });
            }
        }
    }
}