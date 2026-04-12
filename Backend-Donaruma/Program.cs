using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

// 👇 1. NUEVOS IMPORTS PARA LA SEGURIDAD (El diccionario del cadenero) 👇
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("PermitirWeb");
app.UseHttpsRedirection();

// 👇 3. EL ORDEN SAGRADO (ESTO CURA EL ERROR QUE TENÍAS) 👇
app.UseAuthentication(); // PRIMERO verifica la identidad (el Token)
app.UseAuthorization();  // LUEGO verifica los permisos (Roles)

app.MapControllers();
app.Run();