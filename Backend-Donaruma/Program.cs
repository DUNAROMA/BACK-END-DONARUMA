using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddSingleton(new PostgreSQLConfiguration(connectionString!));
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPerfumeService, PerfumeService>();
// Program.cs
builder.Services.AddScoped<IAuthService, AuthService>();


// ?? 1. AQUÍ AGREGAMOS LA CONFIGURACIÓN DE CORS ??

// 1. AQUÍ AGREGAMOS LA CONFIGURACIÓN DE CORS (CORREGIDA)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
    {
        // Quitamos AllowAnyOrigin() y ponemos la ruta EXACTA de tu Angular
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. ACTIVAMOS EL CORS EN EL ORDEN CORRECTO
app.UseRouting(); // <-- Es muy buena práctica poner UseRouting antes de UseCors

app.UseCors("PermitirWeb");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();