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
        public async Task<long> CrearUsuario(CrearUsuarioDTO usuario, string tokenConfirmacion)
        {
            using var db = Connection;

            // 1. Ejecutamos tu función original intacta
            var query = @"SELECT public.crear_usuario(
                @Nombre,
                @Apellidos,
                @Direccion,
                @Correo,
                @Contrasena,
                @Rol)";

            var id = await db.ExecuteScalarAsync<long>(query, usuario);

            // 2. 🔒 INYECTAMOS EL TOKEN: Buscamos al usuario recién creado y le guardamos su llave
            var updateQuery = @"
        UPDATE ""usuarios"" 
        SET ""tokenconfirmacion"" = @Token 
        WHERE ""idusuario"" = @Id;";

            await db.ExecuteAsync(updateQuery, new { Token = tokenConfirmacion, Id = id });

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



        public async Task<bool> ConfirmarCuenta(string token)
        {
            using var db = Connection;

            // Todo en minúsculas para que coincida con tu pgAdmin
            var sql = @"
        UPDATE ""usuarios"" 
        SET ""correoconfirmado"" = true, 
            ""tokenconfirmacion"" = NULL 
        WHERE ""tokenconfirmacion"" = @Token 
        RETURNING ""idusuario"";";

            var idUsuarioActualizado = await db.ExecuteScalarAsync<int?>(sql, new { Token = token });

            return idUsuarioActualizado.HasValue;
        }
    }
}