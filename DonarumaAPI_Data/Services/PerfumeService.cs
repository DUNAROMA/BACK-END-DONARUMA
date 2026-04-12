<<<<<<< HEAD
using Dapper;
using Npgsql;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.PerfumesDTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.PerfumesDTOs;
using Npgsql; // Librería para PostgreSQL
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619

namespace DonarumaAPI_Data.Services
{
    public class PerfumeService : IPerfumeService
    {
<<<<<<< HEAD
        private readonly PostgreSQLConfiguration _connection;

        private const string SelectPerfume = @"
            SELECT idperfume AS IdPerfume, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                   descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                   genero AS Genero, stock AS Stock, intensidad AS Intensidad, 
                   dulzor AS Dulzor, duracion AS Duracion, aromatico AS Aromatico 
            FROM ";

=======
        // 1. Traemos la configuración de PostgreSQL 
        private readonly PostgreSQLConfiguration _connection;

>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        public PerfumeService(PostgreSQLConfiguration connection)
        {
            _connection = connection;
        }

<<<<<<< HEAD
        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(_connection.ConnectionString);
        }

        // --- FUNCIÓN 1: OBTENER TODOS ---
        public async Task<List<PerfumeDTO>> ObtenerTodos()
        {
            using var db = dbConnection();
            var result = await db.QueryAsync<PerfumeDTO>(SelectPerfume + "obtener_todos_perfumes()");
            var lista = result.ToList();

            foreach (var p in lista)
            {
                var sqlFamilias = "SELECT idfamilia FROM perfume_familia WHERE idperfume = @Id";
                p.FamiliasOlfativasIds = (await db.QueryAsync<int>(sqlFamilias, new { Id = p.IdPerfume })).ToList();
            }

            return lista;
=======
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
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        }

        // --- FUNCIÓN 2: OBTENER DE NOCHE ---
        public async Task<List<PerfumeDTO>> ObtenerDeNoche()
        {
<<<<<<< HEAD
            using var db = dbConnection();
            var result = await db.QueryAsync<PerfumeDTO>(SelectPerfume + "obtener_perfumes_noche()");
            return result.ToList();
=======
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
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        }

        // --- FUNCIÓN 3: POR OCASIÓN ---
        public async Task<List<PerfumeDTO>> ObtenerPorOcasion(string ocasion)
        {
<<<<<<< HEAD
            using var db = dbConnection();
            var result = await db.QueryAsync<PerfumeDTO>(
                SelectPerfume + "obtener_perfumes_por_ocasion(@ocasion)", new { ocasion });
            return result.ToList();
=======
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
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        }

        // --- FUNCIÓN 4: POR GÉNERO ---
        public async Task<List<PerfumeDTO>> ObtenerPorGenero(string genero)
        {
<<<<<<< HEAD
            using var db = dbConnection();
            var result = await db.QueryAsync<PerfumeDTO>(
                SelectPerfume + "obtener_perfumes_por_genero(@genero)", new { genero });
            return result.ToList();
=======
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
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        }

        // --- FUNCIÓN 5: CREAR PERFUME ---
        public async Task<int> CrearPerfume(PerfumeDTO perfume)
        {
<<<<<<< HEAD
            using var db = dbConnection();
            var sql = @"INSERT INTO perfumes (nombreperfume, marca, genero, ocasion, precio, descripcion, imagen_url, stock, intensidad, dulzor, duracion, aromatico) 
                VALUES (@Nombre, @Marca, @Genero, @Ocasion, @Precio, @Descripcion, @Imagen_Url, @Stock, @Intensidad, @Dulzor, @Duracion, @Aromatico) 
                RETURNING idperfume";

            var idNuevo = await db.ExecuteScalarAsync<int>(sql, perfume);

            if (perfume.FamiliasOlfativasIds != null && perfume.FamiliasOlfativasIds.Any())
            {
                foreach (var idFam in perfume.FamiliasOlfativasIds)
                {
                    await db.ExecuteAsync(
                        "INSERT INTO perfume_familia (idperfume, idfamilia) VALUES (@IdPerfume, @IdFamilia)",
                        new { IdPerfume = idNuevo, IdFamilia = idFam });
                }
            }

            return idNuevo;
        }

        // --- FUNCIÓN 6: ACTUALIZAR PERFUME ---
        public async Task<bool> ActualizarPerfume(PerfumeDTO perfume)
        {
            using var db = dbConnection();
            var sql = @"UPDATE perfumes 
                SET nombreperfume = @Nombre, 
                    marca         = @Marca, 
                    genero        = @Genero, 
                    ocasion       = @Ocasion, 
                    precio        = @Precio, 
                    descripcion   = @Descripcion, 
                    imagen_url    = @Imagen_Url, 
                    stock         = @Stock,
                    intensidad    = @Intensidad,
                    dulzor        = @Dulzor,
                    duracion      = @Duracion,
                    aromatico     = @Aromatico
                WHERE idperfume = @IdPerfume";

            var filasAfectadas = await db.ExecuteAsync(sql, perfume);

            if (filasAfectadas > 0)
            {
                await db.ExecuteAsync(
                    "DELETE FROM perfume_familia WHERE idperfume = @IdPerfume",
                    new { IdPerfume = perfume.IdPerfume });

                if (perfume.FamiliasOlfativasIds != null && perfume.FamiliasOlfativasIds.Any())
                {
                    foreach (var idFam in perfume.FamiliasOlfativasIds)
                    {
                        await db.ExecuteAsync(
                            "INSERT INTO perfume_familia (idperfume, idfamilia) VALUES (@IdPerfume, @IdFamilia)",
                            new { IdPerfume = perfume.IdPerfume, IdFamilia = idFam });
                    }
                }
            }

            return filasAfectadas > 0;
        }

        // --- FUNCIÓN 7: ELIMINAR PERFUME ---
        public async Task<bool> EliminarPerfume(int idPerfume)
        {
            using var db = dbConnection();
            var sql = "DELETE FROM perfumes WHERE idperfume = @IdPerfume";
            var filasAfectadas = await db.ExecuteAsync(sql, new { IdPerfume = idPerfume });
            return filasAfectadas > 0;
        }

        // --- FUNCIÓN 8: PRECIO MAYOR ---
        public async Task<IEnumerable<PerfumeDTO>> ObtenerPorPrecioMayorAsync()
        {
            using var db = dbConnection();
            return await db.QueryAsync<PerfumeDTO>(SelectPerfume + "obtener_perfumes_precio_mayor()");
        }

        // --- FUNCIÓN 9: PRECIO MENOR ---
        public async Task<IEnumerable<PerfumeDTO>> ObtenerPorPrecioMenorAsync()
        {
            using var db = dbConnection();
            return await db.QueryAsync<PerfumeDTO>(SelectPerfume + "obtener_perfumes_precio_menor()");
        }

        // --- FUNCIÓN 10: BUSCAR POR NOMBRE ---
        public async Task<IEnumerable<PerfumeDTO>> BuscarPorNombreAsync(string nombre)
        {
            using var db = dbConnection();
            return await db.QueryAsync<PerfumeDTO>(
                SelectPerfume + "buscar_perfumes_por_nombre(@nombre)", new { nombre });
        }

        // --- FUNCIÓN 11: OBTENER POR MARCA ---
        public async Task<IEnumerable<PerfumeDTO>> ObtenerPorMarcaAsync(string marca)
        {
            using var db = dbConnection();
            return await db.QueryAsync<PerfumeDTO>(
                SelectPerfume + "obtener_perfumes_por_marca(@marca)", new { marca });
        }
    }
}
=======
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
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
