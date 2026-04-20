using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Services
{
    public class SecurityLogService : ISecurityLogService
    {
        private readonly string _connectionString;

        public SecurityLogService(PostgreSQLConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

       
        public async Task<IEnumerable<SecurityLog>> ObtenerLogsFiltrados(string? buscar, int limite)
        {
            using var db = Connection;

            
            if (string.IsNullOrEmpty(buscar))
            {
                var queryBasica = "SELECT * FROM \"SecurityLogs\" ORDER BY \"EventDate\" DESC LIMIT @Limite;";
                return await db.QueryAsync<SecurityLog>(queryBasica, new { Limite = limite });
            }

           
            var queryFiltrada = @"
                SELECT * FROM ""SecurityLogs"" 
                WHERE ""UserEmail"" ILIKE @Busqueda 
                   OR ""EventType"" ILIKE @Busqueda 
                ORDER BY ""EventDate"" DESC 
                LIMIT @Limite;";

            return await db.QueryAsync<SecurityLog>(queryFiltrada, new
            {
                Busqueda = $"%{buscar}%",
                Limite = limite
            });
        }

       
        public async Task RegistrarEvento(string correoUsuario, string tipoEvento, string direccionIp)
        {
            using var db = Connection;

            var query = @"
                INSERT INTO ""SecurityLogs"" (""UserEmail"", ""EventDate"", ""EventType"", ""IpAddress"") 
                VALUES (@Email, @Fecha, @Evento, @Ip);";

            await db.ExecuteAsync(query, new
            {
                Email = correoUsuario,
                Fecha = DateTime.UtcNow, 
                Evento = tipoEvento,
                Ip = string.IsNullOrEmpty(direccionIp) ? "0.0.0.0" : direccionIp
            });
        }
    }
}