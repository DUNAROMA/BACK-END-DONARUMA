using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
using DonarumaAPI_Data.Data;
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



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ?? 1. AQUÍ AGREGAMOS LA CONFIGURACIÓN DE CORS ??

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
    {
        policy.AllowAnyOrigin()
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


// ?? 2. AQUÍ ACTIVAMOS EL CORS (Debe ir antes de MapControllers) ??

app.UseCors("PermitirWeb");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();