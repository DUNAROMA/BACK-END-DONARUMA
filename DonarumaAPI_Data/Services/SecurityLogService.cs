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

        // --- 1. FUNCIÓN PARA LEER (Con buscador y límite dinámico) ---
        public async Task<IEnumerable<SecurityLog>> ObtenerLogsFiltrados(string? buscar, int limite)
        {
            using var db = Connection;

            // Si el buscador está vacío, solo traemos los últimos X registros
            if (string.IsNullOrEmpty(buscar))
            {
                var queryBasica = "SELECT * FROM \"SecurityLogs\" ORDER BY \"EventDate\" DESC LIMIT @Limite;";
                return await db.QueryAsync<SecurityLog>(queryBasica, new { Limite = limite });
            }

            // Si el usuario escribió algo, buscamos coincidencias (ILIKE no distingue mayúsculas/minúsculas)
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

        // --- 2. FUNCIÓN PARA ESCRIBIR (La "pluma" auditora) ---
        public async Task RegistrarEvento(string correoUsuario, string tipoEvento, string direccionIp)
        {
            using var db = Connection;

            var query = @"
                INSERT INTO ""SecurityLogs"" (""UserEmail"", ""EventDate"", ""EventType"", ""IpAddress"") 
                VALUES (@Email, @Fecha, @Evento, @Ip);";

            await db.ExecuteAsync(query, new
            {
                Email = correoUsuario,
                Fecha = DateTime.UtcNow, // Siempre guardamos la hora universal (UTC)
                Evento = tipoEvento,
                Ip = string.IsNullOrEmpty(direccionIp) ? "0.0.0.0" : direccionIp
            });
        }
    }
}