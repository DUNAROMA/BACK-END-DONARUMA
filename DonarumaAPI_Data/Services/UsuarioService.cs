using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.UsuarioDTo;
using Npgsql;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        // ====================================================================
        // 👇 MÉTODOS DE ALAN INTEGRADOS CORRECTAMENTE DENTRO DE LA CLASE 👇
        // ====================================================================

        public async Task<bool> ActualizarUsuario(ActualizarUsuarioDTO usuario)
        {
            using var db = Connection;

            // Hacemos el UPDATE directo a la tabla usuarios
            var query = @"UPDATE public.usuarios 
                           SET nombre = @Nombre, 
                               apellidos = @Apellidos, 
                               direccion = @Direccion 
                           WHERE idusuario = @IdUsuario;";

            // ExecuteAsync devuelve el número de filas afectadas
            var filasAfectadas = await db.ExecuteAsync(query, usuario);

            // Si afectó 1 o más filas, fue exitoso
            return filasAfectadas > 0;
        }

        public async Task<(bool Exito, string Mensaje)> CambiarPassword(CambiarPasswordDTO datos)
        {
            using var db = Connection;

            // 1. Obtenemos el hash actual de la base de datos
            var queryObtener = "SELECT contrasena FROM public.usuarios WHERE idusuario = @IdUsuario";
            var hashActual = await db.QueryFirstOrDefaultAsync<string>(queryObtener, new { IdUsuario = datos.IdUsuario });

            if (hashActual == null)
                return (false, "Usuario no encontrado.");

            // 2. Verificamos que la contraseña "actual" que escribió, coincida con la de la BD
            bool esValida = BCrypt.Net.BCrypt.Verify(datos.ContrasenaActual, hashActual);

            if (!esValida)
                return (false, "La contraseña actual es incorrecta.");

            // 3. Encriptamos la NUEVA contraseña
            var nuevoHash = BCrypt.Net.BCrypt.HashPassword(datos.ContrasenaNueva);

            // 4. Guardamos el nuevo hash en la base de datos
            var queryUpdate = "UPDATE public.usuarios SET contrasena = @NuevaContrasena WHERE idusuario = @IdUsuario";
            var filasAfectadas = await db.ExecuteAsync(queryUpdate, new { NuevaContrasena = nuevoHash, IdUsuario = datos.IdUsuario });

            if (filasAfectadas > 0)
                return (true, "Contraseña actualizada correctamente.");

            return (false, "Ocurrió un error al guardar la nueva contraseña.");
        }
    }
}