using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.UsuarioDTo;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Donaruma.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDTO usuario)
        {
            var id = await _usuarioService.CrearUsuario(usuario);

            return Ok(new
            {
                mensaje = "Usuario creado",
                idUsuario = id
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuario(long id)
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorId(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }
    }
}
