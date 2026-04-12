using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.PerfumesDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesController : ControllerBase
    {
        private readonly IPerfumeService _perfumeService;

<<<<<<< HEAD
=======
        // Conectamos el controlador con el servicio de Alan
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        public PerfumesController(IPerfumeService perfumeService)
        {
            _perfumeService = perfumeService;
        }

<<<<<<< HEAD
        // GET api/perfumes/todos
=======
        // Ruta: GET api/perfumes/todos
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        [HttpGet("todos")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerTodos()
        {
            var perfumes = await _perfumeService.ObtenerTodos();
            return Ok(perfumes);
        }

<<<<<<< HEAD
        // GET api/perfumes/noche
=======
        // Ruta: GET api/perfumes/noche
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        [HttpGet("noche")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerDeNoche()
        {
            var perfumes = await _perfumeService.ObtenerDeNoche();
            return Ok(perfumes);
        }

<<<<<<< HEAD
        // GET api/perfumes/ocasion/casual
=======
        // Ruta: GET api/perfumes/ocasion/casual
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        [HttpGet("ocasion/{ocasion}")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerPorOcasion(string ocasion)
        {
            var perfumes = await _perfumeService.ObtenerPorOcasion(ocasion);
            return Ok(perfumes);
        }

<<<<<<< HEAD
        // GET api/perfumes/genero/hombre
=======
        // Ruta: GET api/perfumes/genero/hombre
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        [HttpGet("genero/{genero}")]
        public async Task<ActionResult<List<PerfumeDTO>>> ObtenerPorGenero(string genero)
        {
            var perfumes = await _perfumeService.ObtenerPorGenero(genero);
            return Ok(perfumes);
        }

<<<<<<< HEAD
        // GET api/perfumes/marca/chanel
        [HttpGet("marca/{marca}")]
        public async Task<IActionResult> ObtenerPorMarca(string marca)
        {
            var perfumes = await _perfumeService.ObtenerPorMarcaAsync(marca);
            return Ok(perfumes);
        }

        // GET api/perfumes/buscar/noir
        [HttpGet("buscar/{nombre}")]
        public async Task<IActionResult> BuscarPorNombre(string nombre)
        {
            var perfumes = await _perfumeService.BuscarPorNombreAsync(nombre);
            return Ok(perfumes);
        }

        // GET api/perfumes/precio-mayor
        [HttpGet("precio-mayor")]
        public async Task<IActionResult> ObtenerPorPrecioMayor()
        {
            var perfumes = await _perfumeService.ObtenerPorPrecioMayorAsync();
            return Ok(perfumes);
        }

        // GET api/perfumes/precio-menor
        [HttpGet("precio-menor")]
        public async Task<IActionResult> ObtenerPorPrecioMenor()
        {
            var perfumes = await _perfumeService.ObtenerPorPrecioMenorAsync();
            return Ok(perfumes);
        }

        // POST api/perfumes/crear
=======
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
        [HttpPost("crear")]
        public async Task<IActionResult> CrearPerfume([FromBody] PerfumeDTO perfume)
        {
            try
            {
                var nuevoId = await _perfumeService.CrearPerfume(perfume);
                return Ok(new { mensaje = "¡Perfume creado con éxito!", id = nuevoId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear el perfume: " + ex.Message);
            }
        }
<<<<<<< HEAD

        // PUT api/perfumes/actualizar/5
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarPerfume(int id, [FromBody] PerfumeDTO perfume)
        {
            try
            {
                if (id != perfume.IdPerfume)
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el del perfume." });

                var fueEditado = await _perfumeService.ActualizarPerfume(perfume);

                if (fueEditado)
                    return Ok(new { mensaje = "¡Perfume actualizado con éxito!" });

                return NotFound(new { mensaje = "No se encontró el perfume para actualizar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar el perfume: " + ex.Message);
            }
        }

        // DELETE api/perfumes/eliminar/5
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarPerfume(int id)
        {
            try
            {
                var fueBorrado = await _perfumeService.EliminarPerfume(id);

                if (fueBorrado)
                    return Ok(new { mensaje = "¡Perfume eliminado permanentemente!" });

                return NotFound(new { mensaje = "No se encontró el perfume para eliminar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al eliminar el perfume: " + ex.Message);
            }
        }
=======
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
    }
}