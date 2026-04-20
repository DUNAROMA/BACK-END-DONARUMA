using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
using DonarumaAPI_DTOs.UsuarioDTo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEmailService _emailService;

        
        private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", RegexOptions.Compiled);
        private static readonly Regex NameRegex = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", RegexOptions.Compiled);

        public UsuariosController(IUsuarioService usuarioService, IEmailService emailService)
        {
            _usuarioService = usuarioService;
            _emailService = emailService;
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

            
            if (string.IsNullOrWhiteSpace(usuario.Nombre) || string.IsNullOrWhiteSpace(usuario.Apellidos) ||
                !NameRegex.IsMatch(usuario.Nombre) || !NameRegex.IsMatch(usuario.Apellidos))
                return BadRequest(new { mensaje = "El nombre y los apellidos solo deben contener letras." });

            
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            
            string tokenConfirmacion = Guid.NewGuid().ToString();

            
            var id = await _usuarioService.CrearUsuario(usuario, tokenConfirmacion);

            
            await _emailService.EnviarCorreoConfirmacion(usuario.Correo, tokenConfirmacion);

            
            return Ok(new
            {
                mensaje = "Usuario creado con éxito. Por favor revisa tu correo para confirmar tu cuenta.",
                idUsuario = id
            });
        }

        


        
        [HttpPost("confirmar")]
        public async Task<IActionResult> ConfirmarCuenta([FromBody] ConfirmarCuentaRequest request)
        {
            
            var resultado = await _usuarioService.ConfirmarCuenta(request.Token);

            if (!resultado)
                return BadRequest(new { mensaje = "El código es inválido o ya fue utilizado." });

            return Ok(new { mensaje = "¡Cuenta confirmada con éxito! Ya puedes iniciar sesión." });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuario(long id)
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorId(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        
        
        
        [Authorize]
        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarUsuarioDTO usuarioDto)
        {
            
           
            
            var idUsuarioTokenString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                    ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(idUsuarioTokenString))
            {
                return Unauthorized(new { exito = false, mensaje = "Sesión inválida o expirada." });
            }

            int idUsuarioReal = int.Parse(idUsuarioTokenString);

            
            if (usuarioDto.IdUsuario != idUsuarioReal)
            {
                
                return StatusCode(403, new
                {
                    exito = false,
                    mensaje = "Acceso denegado. No tienes permiso para modificar este perfil."
                });
            }

           
            var exito = await _usuarioService.ActualizarUsuario(usuarioDto);

            if (exito)
            {
                return Ok(new { exito = true, mensaje = "Perfil y dirección actualizados correctamente." });
            }

            return BadRequest(new { exito = false, mensaje = "No se pudo actualizar el perfil." });
        }
        [Authorize] 
        [HttpPut("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDTO datos)
        {
            
            var idUsuarioTokenString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                    ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(idUsuarioTokenString))
            {
                return Unauthorized(new { exito = false, mensaje = "Sesión inválida o expirada." });
            }

            int idUsuarioReal = int.Parse(idUsuarioTokenString);

            //  Prevención IDOR 
            if (datos.IdUsuario != idUsuarioReal)
            {
                return StatusCode(403, new
                {
                    exito = false,
                    mensaje = "Acceso denegado. No tienes permiso para modificar esta contraseña."
                });
            }

            
            if (string.IsNullOrWhiteSpace(datos.ContrasenaNueva) || !PasswordRegex.IsMatch(datos.ContrasenaNueva))
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "La nueva contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un carácter especial."
                });
            }

           
            var resultado = await _usuarioService.CambiarPassword(datos);

            if (resultado.Exito)
            {
                return Ok(new { exito = true, mensaje = resultado.Mensaje });
            }

            
            return BadRequest(new { exito = false, mensaje = resultado.Mensaje });
        }
    }
}

public class ConfirmarCuentaRequest
{
    public string Token { get; set; } = string.Empty;
}