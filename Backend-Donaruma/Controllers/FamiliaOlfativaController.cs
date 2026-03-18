using DonarumaAPI_Data.Data;
using DonarumaAPI_DTOs.FamiliaOlfativa;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamiliaOlfativaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FamiliaOlfativaController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint para listar todos los perfumes con su familia
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<FamiliaOlfativaDTO>>> GetTodosLosPerfumes()
        {
            // Usamos el DbSet que acabamos de crear en AppDbContext
            var perfumes = await _context.PerfumesCompletos
                .FromSqlRaw("SELECT * FROM obtener_perfumes_completos();")
                .ToListAsync();

            if (perfumes == null || !perfumes.Any())
            {
                return NotFound("No se encontraron perfumes en el catálogo.");
            }

            return Ok(perfumes);
        }
    }
}