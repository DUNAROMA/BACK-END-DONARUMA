
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
            using var db = dbConnection();
            var sql = "SELECT * FROM obtener_todos_perfumes()";
            var result = await db.QueryAsync<PerfumeDTO>(sql);
            return result.ToList();
        }

        // --- FUNCIÓN 2: OBTENER DE NOCHE ---
        public async Task<List<PerfumeDTO>> ObtenerDeNoche()
        {
            using var db = dbConnection();
            var sql = "SELECT * FROM obtener_perfumes_noche()";
            var result = await db.QueryAsync<PerfumeDTO>(sql);
            return result.ToList();
        }

        // --- FUNCIÓN 3: POR OCASIÓN ---
        public async Task<List<PerfumeDTO>> ObtenerPorOcasion(string ocasion)
        {
            using var db = dbConnection();
            var sql = "SELECT * FROM obtener_perfumes_por_ocasion(@ocasion)";
            var result = await db.QueryAsync<PerfumeDTO>(sql, new { ocasion });
            return result.ToList();
        }

        // --- FUNCIÓN 4: POR GÉNERO ---
        public async Task<List<PerfumeDTO>> ObtenerPorGenero(string genero)
        {
            using var db = dbConnection();
            var sql = "SELECT * FROM obtener_perfumes_por_genero(@genero)";
            var result = await db.QueryAsync<PerfumeDTO>(sql, new { genero });
            return result.ToList();
        }

        // --- FUNCIÓN 5: CREAR PERFUME ---
        public async Task<int> CrearPerfume(PerfumeDTO perfume)
        {
            using var db = dbConnection();
            var sql = @"INSERT INTO perfumes (nombreperfume, marca, genero, ocasion, precio, descripcion, imagen_url, stock) 
                VALUES (@Nombre, @Marca, @Genero, @Ocasion, @Precio, @Descripcion, @Imagen_Url, @Stock) 
                RETURNING idperfume";
            var id = await db.ExecuteScalarAsync<int>(sql, perfume);
            return id;
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
