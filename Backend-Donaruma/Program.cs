using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

// 👇 IMPORTS DE SEGURIDAD Y RATE LIMITING 👇
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System; // Agregado para usar Uri()

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================================================================
// 👇 1. EL TRADUCTOR DE RAILWAY A C# (Arregla el error de Base de Datos)
// ========================================================================
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Convertimos el link raro de Railway al formato estricto que C# exige
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Disable;";
}
else
{
    // Si estamos en tu compu local, usamos el appsettings
    connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
}

builder.Services.AddSingleton(new PostgreSQLConfiguration(connectionString!));

// Inyección de dependencias
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<INovedadService, NovedadService>();
builder.Services.AddScoped<IPerfumeService, PerfumeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddHostedService<LogCleanupService>(); // 👈 Respetamos que lo tienes comentado
builder.Services.AddScoped<IOfertasService, OfertasService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<IEmailService, GmailEmailService>();

// ========================================================================
// 👇 2. AJUSTE DE CORS: Preparado para localhost y donarumastore.com
// ========================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaCors", app =>
    {
        app.WithOrigins(
            "http://localhost:4200",
            "https://www.donarumastore.com",
            "https://donarumastore.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials(); // 👈 La llave mágica de las Cookies
    });
});

// 👇 CONFIGURACIÓN DEL CADENERO ANTI-SPAM (RATE LIMITING) 👇
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PoliticaRegistro", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3, // Solo 3 intentos permitidos...
                Window = TimeSpan.FromMinutes(25), // ...cada 25 minutos
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Se han detectado demasiados intentos de registro. Por motivos de seguridad, por favor espera 25 minutos.", token);
    };
});

// 👇 CONFIGURACIÓN DE TOKENS JWT 👇
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Es mejor poner esto arriba
app.UseRouting(); // 1. Primero sabe a dónde va la petición

// 👇 AQUÍ LLAMAMOS AL CADENERO CON EL NOMBRE CORRECTO ("PoliticaCors") 👇
app.UseCors("PoliticaCors");

// 👇 ACTIVAMOS EL ESCUDO ANTI-SPAM 👇
app.UseRateLimiter();

// 👇 EL ORDEN SAGRADO (ESTO CURA LOS ERRORES DE AUTORIZACIÓN) 👇
app.UseAuthentication(); // PRIMERO verifica la identidad
app.UseAuthorization();  // LUEGO verifica los permisos

app.MapControllers();

app.Run();