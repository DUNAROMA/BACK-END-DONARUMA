using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.UsuarioDTo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.RegularExpressions;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        // 👇 Definimos las Regex de forma estática y compilada (Universal para todo .NET)
        private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", RegexOptions.Compiled);
        private static readonly Regex NameRegex = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", RegexOptions.Compiled);

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("crear")]
        [EnableRateLimiting("PoliticaRegistro")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDTO usuario)
        {
            // Validar Correo
            if (string.IsNullOrWhiteSpace(usuario.Correo) || !EmailRegex.IsMatch(usuario.Correo))
                return BadRequest(new { mensaje = "El formato del correo electrónico es inválido." });

            // Validar Contraseña Segura
            if (string.IsNullOrWhiteSpace(usuario.Contrasena) || !PasswordRegex.IsMatch(usuario.Contrasena))
                return BadRequest(new { mensaje = "La contraseña no cumple con los requisitos de seguridad." });

            // Validar Nombre y Apellidos
            if (string.IsNullOrWhiteSpace(usuario.Nombre) || string.IsNullOrWhiteSpace(usuario.Apellidos) ||
                !NameRegex.IsMatch(usuario.Nombre) || !NameRegex.IsMatch(usuario.Apellidos))
                return BadRequest(new { mensaje = "El nombre y los apellidos solo deben contener letras." });

            // Encriptamos la contraseña con BCrypt
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            // Guardamos en la base de datos
            var id = await _usuarioService.CrearUsuario(usuario);

            return Ok(new
            {
                mensaje = "Usuario creado con éxito",
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