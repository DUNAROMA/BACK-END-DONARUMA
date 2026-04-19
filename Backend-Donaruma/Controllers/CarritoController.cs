using DonarumaAPI_Data.Services;
using Microsoft.AspNetCore.Mvc;

using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.CarritoDTOs;


namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarItem([FromBody] AgregarAlCarritoDTO item)
        {
            if (item == null) return BadRequest();
            var result = await _carritoService.AgregarAlCarrito(item);
            return Ok(result);
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> GetCarrito(int idUsuario)
        {
            return Ok(await _carritoService.ObtenerCarritoPorUsuario(idUsuario));
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarItem(int id)
        {
            return Ok(await _carritoService.EliminarItemCarrito(id));
        }
    }
}