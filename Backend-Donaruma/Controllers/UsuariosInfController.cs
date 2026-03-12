using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DonarumaAPI_Data.Data; // Asegúrate de que este sea el namespace de tu AppDbContext
using DonarumaAPI_DTOs.Compras;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosInfController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosInfController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios/datos/1
        [HttpGet("datos/{id}")]
        public async Task<ActionResult<UsuarioDatosDto>> GetDatosUsuario(long id)
        {
            // Ejecuta la función 'obtener_datos_usuario' de tu base de datos
            var resultado = await _context.UsuarioDatosDtos
                .FromSqlRaw("SELECT * FROM obtener_datos_usuario({0}::BIGINT)", id)
                .ToListAsync();

            var usuario = resultado.FirstOrDefault();

            if (usuario == null)
            {
                return NotFound(new { Mensaje = "Usuario no encontrado en la base de datos." });
            }

            return Ok(usuario);
        }
    }
}