using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.UsuarioDTo;
using Npgsql;
using System.Data;

namespace DonarumaAPI_Data.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string _connectionString;

        public UsuarioService(PostgreSQLConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }
        private IDbConnection Connection => new NpgsqlConnection(_connectionString);
        public async Task<long> CrearUsuario(CrearUsuarioDTO usuario)
        {
            using var db = Connection;

            var query = @"SELECT public.crear_usuario(
                        @Nombre,
                        @Apellidos,
                        @Direccion,
                        @Correo,
                        @Contrasena,
                        @Rol)";

            var id = await db.ExecuteScalarAsync<long>(query, usuario);

            return id;
        }

        public async Task<UsuarioDTO> ObtenerUsuarioPorId(long idUsuario)
        {
            using var db = Connection;

            var query = "SELECT * FROM public.obtener_usuario_por_id(@idusuario)";

            var usuario = await db.QueryFirstOrDefaultAsync<UsuarioDTO>(query, new
            {
                idusuario = idUsuario
            });

            return usuario;
        }

    }
}
