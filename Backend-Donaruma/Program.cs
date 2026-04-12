using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using Stripe;

// 👇 1. NUEVOS IMPORTS PARA LA SEGURIDAD (El diccionario del cadenero) 👇
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

=======
using Backend_Donaruma.Services; // Verifica que este sea el namespace de tu AuthService

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN DE SERVICIOS ---
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

<<<<<<< HEAD
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddSingleton(new PostgreSQLConfiguration(connectionString!));

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<INovedadService, NovedadService>();
builder.Services.AddScoped<IPerfumeService, PerfumeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<LogCleanupService>();
builder.Services.AddScoped<IOfertasService, OfertasService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();

=======
// 👇 ARREGLO PARA ERROR CS8604: Evitamos el nulo en la conexión 👇
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL") ?? "";
builder.Services.AddSingleton(new PostgreSQLConfiguration(connectionString));

// Inyección de Dependencias
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPerfumeService, PerfumeService>();
builder.Services.AddScoped<INovedadService, NovedadService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 👇 CONFIGURACIÓN DE CORS 👇
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
    {
<<<<<<< HEAD
        policy.WithOrigins("http://localhost:4200")
=======
        policy.AllowAnyOrigin()
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
<<<<<<< HEAD

// 👇 2. ENTRENANDO AL CADENERO (Configuración de Tokens JWT) 👇
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
        // Ojo aquí: va a buscar una llave secreta en tu appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

var app = builder.Build();

=======

var app = builder.Build();

// --- PIPELINE DE HTTP ---
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

<<<<<<< HEAD
app.UseRouting();
app.UseCors("PermitirWeb");
app.UseHttpsRedirection();

// 👇 3. EL ORDEN SAGRADO (ESTO CURA EL ERROR QUE TENÍAS) 👇
app.UseAuthentication(); // PRIMERO verifica la identidad (el Token)
app.UseAuthorization();  // LUEGO verifica los permisos (Roles)

app.MapControllers();
=======
// IMPORTANTE: UseCors debe ir antes de los mapas de controladores
app.UseCors("PermitirWeb");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
app.Run();