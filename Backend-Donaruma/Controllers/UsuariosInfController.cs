using DonarumaAPI_Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosInfController : ControllerBase
    {
        private readonly IUsuariosInfService _usuariosInfService;

        public UsuariosInfController(IUsuariosInfService usuariosInfService)
        {
            _usuariosInfService = usuariosInfService;
        }

        // GET: api/UsuariosInf/datos/1
        [HttpGet("datos/{id}")]
        public async Task<IActionResult> GetDatosUsuario(long id)
        {
            try
            {
                var usuario = await _usuariosInfService.GetDatosUsuarioAsync(id);

                if (usuario == null)
                {
                    return NotFound(new { Mensaje = "Usuario no encontrado en la base de datos." });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener datos", detalle = ex.Message });
            }
        }
    }
}