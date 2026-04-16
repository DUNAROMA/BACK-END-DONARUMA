using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.UsuarioDTo;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

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
            
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

           
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
        // Agrégalo dentro de public class UsuariosController : Controller
        // debajo de ObtenerUsuario...

        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarUsuarioDTO usuarioDto)
        {
            var exito = await _usuarioService.ActualizarUsuario(usuarioDto);

            if (exito)
            {
                return Ok(new
                {
                    exito = true,
                    mensaje = "Perfil y dirección actualizados correctamente."
                });
            }

            return BadRequest(new
            {
                exito = false,
                mensaje = "No se pudo actualizar el perfil. Verifica la información."
            });
        }
        [HttpPut("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDTO datos)
        {
            var resultado = await _usuarioService.CambiarPassword(datos);

            if (resultado.Exito)
            {
                return Ok(new
                {
                    exito = true,
                    mensaje = resultado.Mensaje
                });
            }

            // Si la contraseña era incorrecta, regresamos un BadRequest con el mensaje
            return BadRequest(new
            {
                exito = false,
                mensaje = resultado.Mensaje
            });
        }

    }
}
