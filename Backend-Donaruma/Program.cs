using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
=======
>>>>>>> origin/Eclud123

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
<<<<<<< HEAD
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
=======

builder.Services.AddSingleton(new PostgreSQLConfiguration(connectionString));
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPerfumeService, PerfumeService>();
>>>>>>> origin/Eclud123

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

<<<<<<< HEAD
// 2. ACTIVAMOS EL CORS EN EL ORDEN CORRECTO
app.UseRouting(); // <-- Es muy buena práctica poner UseRouting antes de UseCors

app.UseCors("PermitirWeb");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
=======
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
>>>>>>> origin/Eclud123
