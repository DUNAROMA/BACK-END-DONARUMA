// Controllers/PagosController.cs
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Backend_Donaruma.Controllers
{
    // ─── 1. LOS MOLDES PARA ENTENDER A ANGULAR ───
    public class ItemCarrito
    {
        public string nombre { get; set; }
        public long precio { get; set; } // Precio en pesos normales (ej. 500)
        public int cantidad { get; set; }
    }

    public class CheckoutRequest
    {
        public List<ItemCarrito> items { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PagosController : ControllerBase
    {
        private readonly IPagosService _pagosService;

        public PagosController(IPagosService pagosService)
        {
            _pagosService = pagosService;
        }

        [HttpPost("crear-sesion")]
        public async Task<IActionResult> CrearSesion([FromBody] CrearSesionDto dto)
        {
            try
            {
                var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (idUsuarioClaim == null)
                    return Unauthorized("No se pudo identificar al usuario.");

                var (sessionId, url) = await _pagosService.CrearSesionAsync(long.Parse(idUsuarioClaim), dto);

                return Ok(new { id = sessionId, url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("confirmar")]
        [AllowAnonymous] // No requiere JWT porque viene de la redirección de Stripe
        public async Task<IActionResult> ConfirmarPago([FromQuery] string session_id)
        {
            try
            {
                var resultado = await _pagosService.ConfirmarPagoAsync(session_id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ─── 3. EL WEBHOOK SE QUEDA EXACTAMENTE IGUAL ───
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = "whsec_b2dd36d6c7533e666855cf4136a3a1c1cbf1195b8794b214f1527bd28684811d";

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                  json,
                  Request.Headers["Stripe-Signature"],
                  endpointSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    System.Console.WriteLine($"\n\n💰 ¡ÉXITO! Se recibió un pago de Stripe para la sesión: {session.Id}\n\n");
                }

                return Ok();
            }
            catch (StripeException e)
            {
                System.Console.WriteLine($"\n\n❌ ERROR DE STRIPE: {e.Message}\n\n");
                return BadRequest();
            }
        }
    }
}