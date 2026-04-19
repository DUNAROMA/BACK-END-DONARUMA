using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.NovedadesDTOs;
using DonarumaAPI_Model.NovedadesModel;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Services
{
    public class NovedadService : INovedadService
    {
        private readonly PostgreSQLConfiguration _connection;

        public NovedadService(PostgreSQLConfiguration connection)
        {
            _connection = connection;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(_connection.ConnectionString);
        }

        // Obtener todos los posts (los más recientes primero)
        public async Task<List<NovedadesDTO>> ObtenerTodas()
        {
            using var db = dbConnection();
            var sql = @"SELECT idnovedad AS IdNovedad, titulo AS Titulo, 
                               descripcion AS Descripcion, imagenurl AS ImagenUrl, 
                               fechapublicacion AS FechaPublicacion 
                        FROM novedades 
                        ORDER BY fechapublicacion DESC";

            var result = await db.QueryAsync<NovedadesDTO>(sql);
            return result.ToList();
        }

        // Crear una nueva publicación
        public async Task<int> CrearNovedad(NovedadesDTO novedad)
        {
            using var db = dbConnection();
            var sql = @"INSERT INTO novedades (titulo, descripcion, imagenurl) 
                        VALUES (@Titulo, @Descripcion, @ImagenUrl) 
                        RETURNING idnovedad";

            return await db.ExecuteScalarAsync<int>(sql, novedad);
        }

        // Editar un post existente
        public async Task<bool> ActualizarNovedad(NovedadesDTO novedad)
        {
            using var db = dbConnection();
            var sql = @"UPDATE novedades 
                        SET titulo = @Titulo, 
                            descripcion = @Descripcion, 
                            imagenurl = @ImagenUrl 
                        WHERE idnovedad = @IdNovedad";

            var filasAfectadas = await db.ExecuteAsync(sql, novedad);
            return filasAfectadas > 0;
        }

        // Eliminar un post
        public async Task<bool> EliminarNovedad(int idNovedad)
        {
            using var db = dbConnection();
            var sql = "DELETE FROM novedades WHERE idnovedad = @IdNovedad";
            var filasAfectadas = await db.ExecuteAsync(sql, new { IdNovedad = idNovedad });
            return filasAfectadas > 0;
        }
        // Buscar una sola publicación por su ID
        public async Task<NovedadesDTO?> ObtenerPorId(int id)
        {
            using var db = dbConnection();
            var sql = @"SELECT idnovedad AS IdNovedad, titulo AS Titulo, 
                       descripcion AS Descripcion, imagenurl AS ImagenUrl, 
                       fechapublicacion AS FechaPublicacion 
                FROM novedades 
                WHERE idnovedad = @Id";

            return await db.QueryFirstOrDefaultAsync<NovedadesDTO>(sql, new { Id = id }); 
        }
        // Eliminar una publicación
        public async Task<bool> Eliminar(int id)
        {
            using var db = dbConnection();
            var sql = "DELETE FROM novedades WHERE idnovedad = @Id";

            var filasAfectadas = await db.ExecuteAsync(sql, new { Id = id });
            return filasAfectadas > 0;
        }
        
        public async Task<bool> Actualizar(int id, NovedadesDTO novedad)
        {
            using var db = dbConnection();

            
            
            var sql = @"UPDATE novedades 
                SET titulo = @Titulo, 
                    descripcion = @Descripcion, 
                    imagenUrl = @ImagenUrl 
                WHERE idnovedad = @Id";

            var filasAfectadas = await db.ExecuteAsync(sql, new
            {
                Titulo = novedad.Titulo,
                Descripcion = novedad.Descripcion,
                ImagenUrl = novedad.ImagenUrl,
                Id = id
            });

            return filasAfectadas > 0;
        }

    }
}
