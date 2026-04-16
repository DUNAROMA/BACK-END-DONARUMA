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
        // Esperamos 30 segundos antes de la primera limpieza 
        // para darle tiempo al servidor de que despierte bien.
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var config = scope.ServiceProvider.GetRequiredService<PostgreSQLConfiguration>();

                    using var db = new NpgsqlConnection(config.ConnectionString);

                    var sql = @"DELETE FROM ""SecurityLogs"" WHERE ""EventDate"" < @FechaLimite";

                    await db.ExecuteAsync(sql, new { FechaLimite = DateTime.UtcNow.AddDays(-7) });

                    Console.WriteLine("✅ Limpieza de logs completada con éxito.");
                }
            }
            catch (Exception ex)
            {
                // 🛡️ ESCUDO: Si falla la conexión, la app NO se apaga. 
                // Solo imprime el error en los logs de Railway para que lo veas.
                Console.WriteLine($"⚠️ Error silencioso en LogCleanupService: {ex.Message}");
            }

            // El proceso se duerme y vuelve a limpiar dentro de 24 horas
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}