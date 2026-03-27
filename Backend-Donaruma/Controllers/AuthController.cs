using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.LoginDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) =>
        _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDTO login, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var respuesta = await _authService.Login(login, ct);

        return respuesta.Exito
            ? Ok(respuesta)
            : Unauthorized(new { mensaje = respuesta.Mensaje });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        RefreshRequestDTO dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest(new { mensaje = "RefreshToken requerido" });

        var respuesta = await _authService.Refresh(dto.RefreshToken, ct);

        return respuesta.Exito
            ? Ok(respuesta)
            : Unauthorized(new { mensaje = respuesta.Mensaje });
    }

    [HttpPost("logout")]
    [Authorize]                          // solo usuarios autenticados
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        // Extrae el id del JWT actual
        var idClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        await _authService.Logout(idUsuario, ct);
        return NoContent();
    }
}