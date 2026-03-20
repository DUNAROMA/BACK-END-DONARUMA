using Microsoft.AspNetCore.Mvc;
using Npgsql;
using DonarumaAPI_Data;
using DonarumaAPI_DTOs.Compras;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosInfController : ControllerBase
    {
        private readonly PostgreSQLConfiguration _config;

        public UsuariosInfController(PostgreSQLConfiguration config)
        {
            _config = config;
        }

        // GET: api/UsuariosInf/datos/1
        [HttpGet("datos/{id}")]
        public async Task<IActionResult> GetDatosUsuario(long id)
        {
            try
            {
                UsuarioDatosDto? usuario = null;

                using var connection = new NpgsqlConnection(_config.ConnectionString);
                await connection.OpenAsync();

                // Ejecutamos la función directamente en PostgreSQL
                using var command = new NpgsqlCommand("SELECT * FROM obtener_datos_usuario(@id)", connection);
                command.Parameters.AddWithValue("id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    usuario = new UsuarioDatosDto
                    {
                        // Asegúrate de que los nombres de las propiedades en UsuarioDatosDto 
                        // coincidan con los nombres de las columnas que devuelve la función en SQL
                        NombreCompleto = reader["nombre_completo"].ToString() ?? "",
                        DireccionUsuario = reader["direccion_usuario"].ToString() ?? ""
                    };
                }

                if (usuario == null)
                {
                    return NotFound(new { Mensaje = "Usuario no encontrado en la base de datos." });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener datos", detalle = ex.Message });
            }
        }
    }
}