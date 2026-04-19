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
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    
    if (databaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        databaseUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Disable;";
    }
    else
    {
        
        connectionString = databaseUrl;
    }
}
else
{
    
    connectionString = builder.Configuration.GetConnectionString("PostgreSQL")!;
}
builder.Services.AddSingleton(new PostgreSQLConfiguration(connectionString!));


builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<INovedadService, NovedadService>();
builder.Services.AddScoped<IPerfumeService, PerfumeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddHostedService<LogCleanupService>(); 
builder.Services.AddScoped<IOfertasService, OfertasService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<IEmailService, ResendEmailService>();

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
        .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PoliticaRegistro", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(25),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Se han detectado demasiados intentos de registro. Por motivos de seguridad, por favor espera 25 minutos.", token);
    };
});

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

    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
    
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("PoliticaCors");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();