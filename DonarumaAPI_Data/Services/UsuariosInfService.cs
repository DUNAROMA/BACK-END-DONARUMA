using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using Npgsql;

namespace DonarumaAPI_Data.Services
{
    public class UsuariosInfService : IUsuariosInfService
    {
        private readonly PostgreSQLConfiguration _connectionConfig;

        public UsuariosInfService(PostgreSQLConfiguration connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        protected NpgsqlConnection dbConnection() => new NpgsqlConnection(_connectionConfig.ConnectionString);

        public async Task<UsuarioDatosDto?> GetDatosUsuarioAsync(long idUsuario)
        {
            using var db = dbConnection();

            // Dapper se encarga de ejecutar la función y mapear el resultado al DTO.
            // Usamos QueryFirstOrDefaultAsync porque esperamos un solo registro o nulo.
            var sql = @"SELECT nombre_completo AS NombreCompleto, 
                               direccion_usuario AS DireccionUsuario 
                        FROM obtener_datos_usuario(@id)";

            return await db.QueryFirstOrDefaultAsync<UsuarioDatosDto>(sql, new { id = idUsuario });
        }
    }
}