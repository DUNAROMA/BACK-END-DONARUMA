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
        public async Task AgregarDireccion(long idUsuario, DireccionDTO dto)
        {
            using var db = Connection;

            await db.ExecuteAsync(
                "SELECT public.agregar_direccion(@idusuario, @direccion)",
                new
                {
                    idusuario = idUsuario,
                    direccion = dto.Direccion
                }
            );
        }

        public async Task<PerfilUsuarioDto> ObtenerPerfilUsuarioAsync(long idUsuario)
        {
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<PerfilUsuarioDto>(
                "SELECT * FROM obtener_perfil_usuario(@p_id)", new { p_id = idUsuario });
        }

        public async Task EditarPerfilUsuarioAsync(long idUsuario, EditarPerfilUsuarioDto dto)
        {
            using var db = Connection;
            await db.ExecuteAsync(
                "SELECT editar_perfil_usuario(@p_id, @p_nom, @p_ape, @p_cor, @p_dir)",
                new { p_id = idUsuario, p_nom = dto.Nombre, p_ape = dto.Apellidos, p_cor = dto.Correo, p_dir = dto.Direccion });
        }

        public async Task<bool> CambiarContrasenaUsuarioAsync(long idUsuario, string contrasenaActual, string nuevaContrasenaHash)
        {
            using var db = Connection;
            return await db.ExecuteScalarAsync<bool>(
                "SELECT cambiar_contrasena_usuario(@p_id, @p_actual, @p_nueva)",
                new { p_id = idUsuario, p_actual = contrasenaActual, p_nueva = nuevaContrasenaHash });
        }
    }
}
