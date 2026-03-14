
﻿using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.PerfumesDTOs;
using Npgsql; // Librería para PostgreSQL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(_connection.ConnectionString);
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
                                Nombre = reader["nombreperfume"]?.ToString() ?? "",
                                Marca = reader["marca"]?.ToString() ?? "",
                                Genero = reader["genero"]?.ToString() ?? "",
                                Ocasion = reader["ocasion"]?.ToString() ?? "",
                                Precio = Convert.ToDecimal(reader["precio"]),
                                Descripcion = reader["descripcion"]?.ToString() ?? "",
                                Imagen_Url = reader["imagen_url"]?.ToString() ?? "",
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
                                Nombre = reader["nombreperfume"]?.ToString() ?? "",
                                Marca = reader["marca"]?.ToString() ?? "",
                                Genero = reader["genero"]?.ToString() ?? "",
                                Ocasion = reader["ocasion"]?.ToString() ?? "",
                                Precio = Convert.ToDecimal(reader["precio"]),
                                Descripcion = reader["descripcion"]?.ToString() ?? "",
                                Imagen_Url = reader["imagen_url"]?.ToString() ?? "",
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
                                Nombre = reader["nombreperfume"]?.ToString() ?? "",
                                Marca = reader["marca"]?.ToString() ?? "",
                                Genero = reader["genero"]?.ToString() ?? "",
                                Ocasion = reader["ocasion"]?.ToString() ?? "",
                                Precio = Convert.ToDecimal(reader["precio"]),
                                Descripcion = reader["descripcion"]?.ToString() ?? "",
                                Imagen_Url = reader["imagen_url"]?.ToString() ?? "",
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

        public async Task<IEnumerable<PerfumeDTO>> ObtenerPorMarcaAsync(string marca)
        {
            using var db = dbConnection();
            var sql = @"SELECT idperfume AS Id, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                               descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                               genero AS Genero, stock AS Stock 
                        FROM perfumes WHERE marca = @Marca";
            return await db.QueryAsync<PerfumeDTO>(sql, new { Marca = marca });
        }


        public async Task<IEnumerable<PerfumeDTO>> ObtenerPorPrecioMayorAsync()
        {
            using var db = dbConnection();
            var sql = @"SELECT idperfume AS Id, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                               descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                               genero AS Genero, stock AS Stock 
                        FROM perfumes ORDER BY precio DESC";
            return await db.QueryAsync<PerfumeDTO>(sql);
        }

        public async Task<IEnumerable<PerfumeDTO>> ObtenerPorPrecioMenorAsync()
        {
            using var db = dbConnection();
            var sql = @"SELECT idperfume AS Id, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                               descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                               genero AS Genero, stock AS Stock 
                        FROM perfumes ORDER BY precio ASC";
            return await db.QueryAsync<PerfumeDTO>(sql);
        }

        public async Task<IEnumerable<PerfumeDTO>> BuscarPorNombreAsync(string nombre)
        {
            using var db = dbConnection();
            var sql = @"SELECT idperfume AS IdPerfume, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                       descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                       genero AS Genero, stock AS Stock 
                FROM perfumes WHERE nombreperfume ILIKE @Nombre";
            return await db.QueryAsync<PerfumeDTO>(sql, new { Nombre = "%" + nombre + "%" });
        }


    }
}
