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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddSingleton(new PostgreSQLConfiguration(connectionString!));

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<INovedadService, NovedadService>();
builder.Services.AddScoped<IPerfumeService, PerfumeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<LogCleanupService>();
builder.Services.AddScoped<IOfertasService, OfertasService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
    {
        // ⚠️ Nota: Asegúrate de agregar aquí también tus dominios de Vercel y DunaromaStore
        // como lo hicimos en el paso anterior.
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 👇 1. CONFIGURACIÓN DEL CADENERO ANTI-SPAM (RATE LIMITING) 👇
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

// 👇 2. CONFIGURACIÓN DE TOKENS JWT 👇
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

app.UseRouting();
app.UseCors("PermitirWeb");

// 👇 3. ACTIVAMOS EL ESCUDO ANTI-SPAM (Debe ir aquí, después de CORS) 👇
app.UseRateLimiter();

app.UseHttpsRedirection();

// 👇 4. EL ORDEN SAGRADO (ESTO CURA LOS ERRORES DE AUTORIZACIÓN) 👇
app.UseAuthentication(); // PRIMERO verifica la identidad
app.UseAuthorization();  // LUEGO verifica los permisos

app.MapControllers();

app.Run();