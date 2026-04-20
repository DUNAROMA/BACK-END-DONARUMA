using Microsoft.AspNetCore.Mvc;
using Npgsql;
using DonarumaAPI_Data;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesStockController : ControllerBase
    {
        private readonly PostgreSQLConfiguration _config;

        public PerfumesStockController(PostgreSQLConfiguration config)
        {
            _config = config;
        }

        [HttpGet("disponibles")]
        public async Task<IActionResult> GetPerfumesDisponibles()
        {
            var perfumes = new List<object>(); 

            using var connection = new NpgsqlConnection(_config.ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM fn_listar_perfumes_disponibles()", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                perfumes.Add(new
                {
                    Id = reader["idperfume"],
                    Nombre = reader["nombreperfume"],
                    Precio = reader["precio"],
                    Stock = reader["stock"]
                });
            }

            return Ok(perfumes);
        }
    }
}