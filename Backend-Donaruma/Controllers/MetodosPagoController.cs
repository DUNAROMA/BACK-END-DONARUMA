using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_DTOs.Compras;
using Npgsql;
using DonarumaAPI_Data;

namespace PerfumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetodosPagoController : ControllerBase
    {
        private readonly PostgreSQLConfiguration _config;

        public MetodosPagoController(PostgreSQLConfiguration config)
        {
            _config = config;
        }

        [HttpGet("buscar/{idusuario}")]
        public async Task<IActionResult> GetPagosFuncion(long idusuario)
        {
            var lista = new List<MetodoPagoDto>();

            using var connection = new NpgsqlConnection(_config.ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM buscar_metodos_pago_usuario(@id)", connection);
            command.Parameters.AddWithValue("id", idusuario);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new MetodoPagoDto
                {
                    // CORRECCIÓN: Agregamos los guiones bajos para que coincidan con SQL
                    NumeroTarjeta = reader["numero_tarjeta"].ToString(),
                    NombreTitular = reader["nombre_titular"].ToString()

                    
                });
            }

            if (!lista.Any()) return NotFound("Sin métodos de pago.");
            return Ok(lista);
        }

        [HttpDelete("eliminar/{idusuario}/{idtarjeta}")]
        public async Task<IActionResult> EliminarPagoFuncion(long idusuario, long idtarjeta)
        {
            using var connection = new NpgsqlConnection(_config.ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT eliminar_metodo_pago_usuario(@u, @t)", connection);
            command.Parameters.AddWithValue("u", idusuario);
            command.Parameters.AddWithValue("t", idtarjeta);

            var mensaje = await command.ExecuteScalarAsync();
            return Ok(new { Respuesta = mensaje?.ToString() });
        }
    }
}