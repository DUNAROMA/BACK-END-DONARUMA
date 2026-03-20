using DonarumaAPI_Data;
using DonarumaAPI_DTOs.FamiliaOlfativa;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamiliaOlfativaController : ControllerBase
    {
        private readonly PostgreSQLConfiguration _config;

        public FamiliaOlfativaController(PostgreSQLConfiguration config)
        {
            _config = config;
        }

        [HttpGet("todos")]
        public async Task<IActionResult> GetTodosLosPerfumes()
        {
            var perfumes = new List<FamiliaOlfativaDTO>();

            try
            {
                using var connection = new NpgsqlConnection(_config.ConnectionString);
                await connection.OpenAsync();

                using var command = new NpgsqlCommand("SELECT * FROM obtener_perfumes_completos();", connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    perfumes.Add(new FamiliaOlfativaDTO
                    {
                        // BIGINT (Nunca nulo, conversión directa)
                        IdPerfume = Convert.ToInt64(reader["idperfume"]),

                        // VARCHAR / TEXT (Validamos si es DBNull antes de convertir a string)
                        Nombre = reader["nombre"] != DBNull.Value ? reader["nombre"].ToString() : null,
                        Marca = reader["marca"] != DBNull.Value ? reader["marca"].ToString() : null,
                        Genero = reader["genero"] != DBNull.Value ? reader["genero"].ToString() : null,
                        Ocasion = reader["ocasion"] != DBNull.Value ? reader["ocasion"].ToString() : null,
                        Descripcion = reader["descripcion"] != DBNull.Value ? reader["descripcion"].ToString() : null,
                        Imagen_Url = reader["imagen_url"] != DBNull.Value ? reader["imagen_url"].ToString() : null,
                        FamiliaOlfativa = reader["familiaolfativa"] != DBNull.Value ? reader["familiaolfativa"].ToString() : null,

                        // NUMERIC (Validamos si es nulo antes de convertir a decimal)
                        Precio = reader["precio"] != DBNull.Value ? Convert.ToDecimal(reader["precio"]) : null,

                        // INTEGER (Validamos si es nulo antes de convertir a int)
                        Stock = reader["stock"] != DBNull.Value ? Convert.ToInt32(reader["stock"]) : null
                    });
                }

                if (!perfumes.Any())
                {
                    return NotFound("No se encontraron perfumes en el catálogo.");
                }

                return Ok(perfumes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}