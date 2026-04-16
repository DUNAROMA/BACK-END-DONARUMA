using Dapper;
using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Model.UsuariosModel;
using DonarumaAPI_DTOs.LoginDTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using PerfumeApi.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public sealed class AuthService : IAuthService
{
    private readonly string _connectionString;
    private readonly IConfiguration _config;

    private IDbConnection Connection => new NpgsqlConnection(_connectionString);

    public AuthService(PostgreSQLConfiguration config, IConfiguration configuration)
    {
        _connectionString = config.ConnectionString;
        _config = configuration;
    }

    // ─── LOGIN ────────────────────────────────────────────────────────────────
    public async Task<LoginResponse> Login(LoginDTO login, CancellationToken ct = default)
    {
        using var db = Connection;

        var usuario = await db.QueryFirstOrDefaultAsync<Usuarios>(
            new CommandDefinition(
                "SELECT * FROM fn_obtener_usuario_por_correo(@correo)",
                new { correo = login.Correo },
                cancellationToken: ct
            )
        );

        // Mismo mensaje para correo y contraseña incorrectos (no revelar cuál falló)
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(login.Contrasena, usuario.Contrasena))
        {
            // --- REGISTRO DE SEGURIDAD: INTENTO FALLIDO ---
            await GuardarLog(db, login.Correo, "Login Fallido", "IP Desconocida", ct);
            return Fallo("Correo o contraseña incorrectos");
        }

        // --- REGISTRO DE SEGURIDAD: ACCESO EXITOSO ---
        await GuardarLog(db, login.Correo, "Login Exitoso", "IP Aprobada", ct);

        return await GenerarRespuestaCompletaAsync(db, usuario, ct);
    }

    // ─── REFRESH ──────────────────────────────────────────────────────────────
    public async Task<LoginResponse> Refresh(string refreshToken, CancellationToken ct = default)
    {
        // El token viaja hasheado en BD — lo hasheamos para comparar
        var hash = HashToken(refreshToken);

        using var db = Connection;

        var usuario = await db.QueryFirstOrDefaultAsync<Usuarios>(
            new CommandDefinition(
                "SELECT * FROM fn_obtener_usuario_por_refresh_token(@token)",
                new { token = hash },
                cancellationToken: ct
            )
        );

        if (usuario is null)
            return Fallo("Refresh token inválido");

        if (usuario.RefreshTokenExpiry < DateTime.UtcNow)
            return Fallo("Refresh token expirado, inicia sesión nuevamente");

        // Rotación: invalida el anterior, emite uno nuevo
        return await GenerarRespuestaCompletaAsync(db, usuario, ct);
    }

    // ─── LOGOUT ───────────────────────────────────────────────────────────────
    public async Task Logout(int idUsuario, CancellationToken ct = default)
    {
        using var db = Connection;

        await db.ExecuteAsync(
            new CommandDefinition(
                "SELECT fn_revocar_refresh_token(@id)",
                new { id = idUsuario },
                cancellationToken: ct
            )
        );
    }

    // ─── PRIVADOS ─────────────────────────────────────────────────────────────
    private async Task<LoginResponse> GenerarRespuestaCompletaAsync(
        IDbConnection db, Usuarios usuario, CancellationToken ct)
    {

        var accessToken = GenerarAccessToken(usuario);
        var refreshToken = GenerarRefreshToken();           // valor plano → va al cliente
        var hashGuardar = HashToken(refreshToken);        // hash      → va a la BD

        var expiry = DateTime.UtcNow.AddDays(
            Convert.ToInt32(_config["JwtSettings:RefreshTokenDays"] ?? "7"));

        // Persiste el hash del refresh token
        await db.ExecuteAsync(
            new CommandDefinition(
                "SELECT fn_guardar_refresh_token(@id, @token, @expiry)",
                new { id = usuario.IdUsuario, token = hashGuardar, expiry },
                cancellationToken: ct
            )
        );

        return new LoginResponse
        {
            Exito = true,
            Mensaje = "Autenticación exitosa",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Rol = usuario.Rol,           // 👈 Agregamos esto
            IdUsuario = usuario.IdUsuario // 👈 Y esto
        };
    }

    private string GenerarAccessToken(Usuarios usuario)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID único
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(               // ✅ UtcNow
                Convert.ToDouble(_config["JwtSettings:DurationInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Refresh token: valor aleatorio criptográficamente seguro
    private static string GenerarRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    // Hash SHA-256 del refresh token antes de guardarlo en BD
    private static string HashToken(string token)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(
            Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }

    private static LoginResponse Fallo(string mensaje) =>
        new() { Exito = false, Mensaje = mensaje };

    // ─── REGISTRO DE AUDITORÍA (OWASP A09) ────────────────────────────────────
    private async Task GuardarLog(IDbConnection db, string email, string tipo, string ip, CancellationToken ct)
    {
        var sql = @"
            INSERT INTO ""SecurityLogs"" (""UserEmail"", ""EventDate"", ""EventType"", ""IpAddress"") 
            VALUES (@UserEmail, @EventDate, @EventType, @IpAddress)";

        await db.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    UserEmail = email,
                    EventDate = DateTime.UtcNow,
                    EventType = tipo,
                    IpAddress = ip
                },
                cancellationToken: ct
            )
        );
    }
}