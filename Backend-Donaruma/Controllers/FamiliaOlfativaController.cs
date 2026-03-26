using DonarumaAPI_Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamiliaOlfativaController : ControllerBase
    {
        private readonly IFamiliaOlfativaService _familiaService;

        public FamiliaOlfativaController(IFamiliaOlfativaService familiaService)
        {
            _familiaService = familiaService;
        }

        [HttpGet("todos")]
        public async Task<IActionResult> GetTodosLosPerfumes()
        {
            try
            {
                var perfumes = await _familiaService.GetTodosLosPerfumesAsync();

                if (perfumes == null || !perfumes.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron perfumes en el catálogo." });
                }

                return Ok(perfumes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}