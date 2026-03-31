using Dapper;
using Npgsql;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.PerfumesDTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Services
{
    public class PerfumeService : IPerfumeService
    {
        private readonly PostgreSQLConfiguration _connection;

        private const string SelectPerfume = @"
            SELECT idperfume AS IdPerfume, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                   descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                   genero AS Genero, stock AS Stock, intensidad AS Intensidad, 
                   dulzor AS Dulzor, duracion AS Duracion, aromatico AS Aromatico 
            FROM ";

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
            var result = await db.QueryAsync<PerfumeDTO>(SelectPerfume + "obtener_todos_perfumes()");
            var lista = result.ToList();

            foreach (var p in lista)
            {
                var sqlFamilias = "SELECT idfamilia FROM perfume_familia WHERE idperfume = @Id";
                p.FamiliasOlfativasIds = (await db.QueryAsync<int>(sqlFamilias, new { Id = p.IdPerfume })).ToList();
            }

            return lista;
        }

        // --- FUNCIÓN 2: OBTENER DE NOCHE ---
        public async Task<List<PerfumeDTO>> ObtenerDeNoche()
        {
            using var db = dbConnection();
            var result = await db.QueryAsync<PerfumeDTO>(SelectPerfume + "obtener_perfumes_noche()");
            return result.ToList();
        }

        // --- FUNCIÓN 3: POR OCASIÓN ---
        public async Task<List<PerfumeDTO>> ObtenerPorOcasion(string ocasion)
        {
            using var db = dbConnection();
            var result = await db.QueryAsync<PerfumeDTO>(
                SelectPerfume + "obtener_perfumes_por_ocasion(@ocasion)", new { ocasion });
            return result.ToList();
        }

        // --- FUNCIÓN 4: POR GÉNERO ---
        public async Task<List<PerfumeDTO>> ObtenerPorGenero(string genero)
        {
            using var db = dbConnection();
            var result = await db.QueryAsync<PerfumeDTO>(
                SelectPerfume + "obtener_perfumes_por_genero(@genero)", new { genero });
            return result.ToList();
        }

        // --- FUNCIÓN 5: CREAR PERFUME ---
        public async Task<int> CrearPerfume(PerfumeDTO perfume)
        {
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