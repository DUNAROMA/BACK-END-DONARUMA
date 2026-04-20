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
                
                Console.WriteLine($"⚠️ Error silencioso en LogCleanupService: {ex.Message}");
            }

            
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}