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
        // 1. Traemos la configuración de PostgreSQL 
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

                // 3. Escribimos la consulta SQL 
                using (var cmd = new NpgsqlCommand("SELECT * FROM obtener_todos_perfumes()", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // 4. Leemos fila por fila y lo convertimos a nuestro DTO
                        while (await reader.ReadAsync())
                        {
                            listaPerfumes.Add(new PerfumeDTO
                            {
                                IdPerfume = Convert.ToInt32(reader["idperfume"]),
                                Nombre = reader["nombreperfume"].ToString(),
                                Marca = reader["marca"].ToString(),
                                Genero = reader["genero"].ToString(),
                                Ocasion = reader["ocasion"].ToString(),
                                Precio = Convert.ToDecimal(reader["precio"]),

                            
                                Descripcion = reader["descripcion"]?.ToString(),
                                Imagen_Url = reader["imagen_url"]?.ToString(),
                                Stock = reader["stock"] != DBNull.Value ? Convert.ToInt32(reader["stock"]) : 0
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

                // Aquí el SQL cambia para filtrar solo los de noche
                string sql = "SELECT * FROM perfumes WHERE ocasion = 'Noche'";

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
            var listaPerfumes = new List<PerfumeDTO>();

            using (var conn = new NpgsqlConnection(_connection.ConnectionString))
            {
                await conn.OpenAsync();

                // 1. Usamos el SQL con un filtro WHERE ocasion = @ocasion
                string sql = "SELECT * FROM obtener_perfumes_por_ocasion(@ocasion)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    // 2. Le pasamos al SQL la palabra que escribiste en Swagger (ej. "Noche")
                    cmd.Parameters.AddWithValue("ocasion", ocasion);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                      
                        while (await reader.ReadAsync())
                        {
                            listaPerfumes.Add(new PerfumeDTO
                            {
                                IdPerfume = Convert.ToInt32(reader["idperfume"]),
                                Nombre = reader["nombreperfume"].ToString(), 
                                Marca = reader["marca"].ToString(),
                                Genero = reader["genero"].ToString(),
                                Ocasion = reader["ocasion"].ToString(),
                                Precio = Convert.ToDecimal(reader["precio"]),                       
                                Descripcion = reader["descripcion"]?.ToString(),
                                Imagen_Url = reader["imagen_url"]?.ToString(),
                                Stock = reader["stock"] != DBNull.Value ? Convert.ToInt32(reader["stock"]) : 0
                            });
                        }
                    }
                }
            }

            return listaPerfumes;
        }

        // --- FUNCIÓN 4: POR GÉNERO ---
        public async Task<List<PerfumeDTO>> ObtenerPorGenero(string genero)
        {
            var listaPerfumes = new List<PerfumeDTO>();

            using (var conn = new NpgsqlConnection(_connection.ConnectionString))
            {
                await conn.OpenAsync();
                string sql = "SELECT * FROM obtener_perfumes_por_genero(@genero)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("genero", genero);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            listaPerfumes.Add(new PerfumeDTO
                            {
                                IdPerfume = Convert.ToInt32(reader["idperfume"]),
                                Nombre = reader["nombreperfume"].ToString(), // Sin mayúsculas
                                Marca = reader["marca"].ToString(),
                                Genero = reader["genero"].ToString(),
                                Ocasion = reader["ocasion"].ToString(),
                                Precio = Convert.ToDecimal(reader["precio"]),

                                // Las 3 columnas nuevas con protección contra nulos (¡y borramos EsDeNoche!)
                                Descripcion = reader["descripcion"]?.ToString(),
                                Imagen_Url = reader["imagen_url"]?.ToString(),
                                Stock = reader["stock"] != DBNull.Value ? Convert.ToInt32(reader["stock"]) : 0
                            });
                        }
                    }
                }
            }
            return listaPerfumes;
        }

        // --- FUNCIÓN 5: CREAR PERFUME ---
        public async Task<int> CrearPerfume(PerfumeDTO perfume)
        {
            using (var conn = new NpgsqlConnection(_connection.ConnectionString))
            {
                await conn.OpenAsync();

                string sql = @"INSERT INTO perfumes (nombreperfume, marca, genero, ocasion, precio, descripcion, imagen_url, stock) 
                       VALUES (@nombre, @marca, @genero, @ocasion, @precio, @descripcion, @imagen, @stock) 
                       RETURNING idperfume;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("nombre", perfume.Nombre);
                    cmd.Parameters.AddWithValue("marca", perfume.Marca);
                    cmd.Parameters.AddWithValue("genero", perfume.Genero);
                    cmd.Parameters.AddWithValue("ocasion", perfume.Ocasion);
                    cmd.Parameters.AddWithValue("precio", perfume.Precio);
                    cmd.Parameters.AddWithValue("descripcion", perfume.Descripcion ?? "");
                    cmd.Parameters.AddWithValue("imagen", perfume.Imagen_Url ?? "");
                    cmd.Parameters.AddWithValue("stock", perfume.Stock);

                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }
    }
}
