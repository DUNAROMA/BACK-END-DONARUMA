using Dapper;
using DonarumaAPI_Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

public class LogCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public LogCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Obtenemos tu configuración de conexión de PostgreSQL
                var config = scope.ServiceProvider.GetRequiredService<PostgreSQLConfiguration>();
                using var db = new NpgsqlConnection(config.ConnectionString);

                // Borramos todo registro de seguridad que tenga más de 7 días
                var sql = @"DELETE FROM ""SecurityLogs"" WHERE ""EventDate"" < @FechaLimite";

                await db.ExecuteAsync(sql, new { FechaLimite = DateTime.UtcNow.AddDays(-7) });
            }

            // El proceso se duerme y vuelve a limpiar dentro de 24 horas
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}