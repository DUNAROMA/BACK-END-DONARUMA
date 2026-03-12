using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using DonarumaAPI_Data.Data;
using DonarumaAPI_DTOs.Compras;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesStockController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PerfumesStockController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint para listar perfumes con stock > 0
        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<Perfume>>> GetPerfumesDisponibles()
        {
            // Al usar FromSqlRaw con la tabla 'perfumes', EF busca las columnas 
            // que definimos arriba con [Column]
            var perfumes = await _context.Perfumes
                .FromSqlRaw("SELECT * FROM fn_listar_perfumes_disponibles()")
                .ToListAsync();

            if (perfumes == null || !perfumes.Any())
            {
                return NotFound("No se encontraron perfumes con stock disponible.");
            }

            return Ok(perfumes);
        }
    }
}