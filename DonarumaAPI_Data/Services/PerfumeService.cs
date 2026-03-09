using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.PerfumesDTOs;
using Npgsql; // Librería para PostgreSQL

namespace DonarumaAPI_Data.Services
{
    public class PerfumeService : IPerfumeService
    {
        // 1. Traemos la configuración de PostgreSQL que hiciste en Program.cs
        private readonly PostgreSQLConfiguration _connection;

        public PerfumeService(PostgreSQLConfiguration connection)
        {
            _connection = connection;
        }

        // --- FUNCIÓN 1: OBTENER TODOS ---
        public async Task<List<PerfumeDTO>> ObtenerTodos()
        {
            var listaPerfumes = new List<PerfumeDTO>();

            // 2. Nos conectamos a la base de datos
            using (var conn = new NpgsqlConnection(_connection.ConnectionString))
            {
                await conn.OpenAsync();

                // 3. Escribimos la consulta SQL (Alan debe ajustar el nombre de la tabla si es distinto)
                using (var cmd = new NpgsqlCommand("SELECT * FROM Perfumes", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // 4. Leemos fila por fila y lo convertimos a nuestro DTO
                        while (await reader.ReadAsync())
                        {
                            listaPerfumes.Add(new PerfumeDTO
                            {
                                IdPerfume = Convert.ToInt32(reader["idPerfume"]),
                                Nombre = reader["nombreperfume"].ToString(),
                                Marca = reader["marca"].ToString(),
                                Genero = reader["genero"].ToString(),
                                Ocasion = reader["ocasion"].ToString(),
                                EsDeNoche = Convert.ToBoolean(reader["esdenoche"]),
                                Precio = Convert.ToDecimal(reader["precio"])
                            });
                        }
                    }
                }
            }

            return listaPerfumes; // Devolvemos la lista al Controlador
        }

        // --- FUNCIÓN 2: OBTENER DE NOCHE ---
        public async Task<List<PerfumeDTO>> ObtenerDeNoche()
        {
            var listaPerfumes = new List<PerfumeDTO>();

            using (var conn = new NpgsqlConnection(_connection.ConnectionString))
            {
                await conn.OpenAsync();

                // Alan: Aquí el SQL cambia para filtrar solo los de noche
                string sql = "SELECT * FROM Perfumes WHERE EsDeNoche = true";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    // ... (Misma lógica de lectura del reader que arriba) ...
                }
            }

            return listaPerfumes;
        }

        // --- FUNCIÓN 3: POR OCASIÓN ---
        public async Task<List<PerfumeDTO>> ObtenerPorOcasion(string ocasion)
        {
            // Alan: Aquí el SQL sería algo como: 
            // "SELECT * FROM Perfumes WHERE Ocasion = @ocasion"
            throw new NotImplementedException();
        }

        // --- FUNCIÓN 4: POR GÉNERO ---
        public async Task<List<PerfumeDTO>> ObtenerPorGenero(string genero)
        {
            // Alan: Aquí el SQL sería algo como: 
            // "SELECT * FROM Perfumes WHERE Genero = @genero"
            throw new NotImplementedException();
        }
    }
}
