using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddSingleton(new PostgreSQLConfiguration(connectionString!));

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPerfumeService, PerfumeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<LogCleanupService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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
app.UseAuthorization();
app.MapControllers();

app.Run(); 