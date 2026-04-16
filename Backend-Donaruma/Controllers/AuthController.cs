using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.LoginDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend_Donaruma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) =>
            _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var respuesta = await _authService.Login(login, ct);

            if (respuesta.Exito)
            {
                // 🔒 Guardamos los tokens en la Bóveda del Navegador
                SetCookiesSecretas(respuesta.AccessToken, respuesta.RefreshToken);

                // 🕵️‍♂️ Extraemos el ID y el Rol directamente del Token para dárselos a Angular
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(respuesta.AccessToken);

                var idUsuario = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var rol = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "cliente";

                // 👇 AHORA SÍ, LE MANDAMOS EL PAQUETE COMPLETO A ANGULAR 👇
                return Ok(new
                {
                    exito = true,
                    mensaje = respuesta.Mensaje,
                    idUsuario = idUsuario,
                    rol = rol
                });
            }

            return Unauthorized(new { mensaje = respuesta.Mensaje });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            // 🔒 Ahora el RefreshToken se lee directamente de la Cookie, no del body
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { mensaje = "No se encontró la credencial de renovación." });

            var respuesta = await _authService.Refresh(refreshToken, ct);

            if (respuesta.Exito)
            {
                SetCookiesSecretas(respuesta.AccessToken, respuesta.RefreshToken);
                return Ok(new { exito = true, mensaje = respuesta.Mensaje });
            }

            return Unauthorized(new { mensaje = respuesta.Mensaje });
        }

        [HttpPost("logout")]
        
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var idClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (int.TryParse(idClaim, out var idUsuario))
            {
                await _authService.Logout(idUsuario, ct);
            }

            // 🗑️ Borramos las cookies de seguridad para cerrar la bóveda
            Response.Cookies.Delete("accessToken", GetCookieOptionsBase());
            Response.Cookies.Delete("refreshToken", GetCookieOptionsBase());

            return NoContent();
        }

        // ─── MÉTODOS PRIVADOS DE SEGURIDAD ─────────────────────────────────────

        private void SetCookiesSecretas(string accessToken, string refreshToken)
        {
            var accessOptions = GetCookieOptionsBase();
            accessOptions.Expires = DateTime.UtcNow.AddMinutes(15); // Debe coincidir con la duración de tu JWT

            var refreshOptions = GetCookieOptionsBase();
            refreshOptions.Expires = DateTime.UtcNow.AddDays(7); // Debe coincidir con la duración de tu Refresh

            Response.Cookies.Append("accessToken", accessToken, accessOptions);
            Response.Cookies.Append("refreshToken", refreshToken, refreshOptions);
        }

        private CookieOptions GetCookieOptionsBase()
        {
            return new CookieOptions
            {
                HttpOnly = true, // 🛡️ JavaScript (y los hackers) NO pueden leer esto
                Secure = true,   // 🛡️ Solo viaja por HTTPS (Obligatorio en producción)
                SameSite = SameSiteMode.None // 🛡️ Permite que Vercel se comunique con Railway
            };
        }
    }
}