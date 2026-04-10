using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.V2.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        // ─── 1. EL ENDPOINT QUE YA TENÍAS (CREAR SESIÓN) ───
        [HttpPost("crear-sesion")]
        public IActionResult CrearSesion([FromBody] object carrito)
        {
            var domain = "http://localhost:4200";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = 92000,
                            Currency = "mxn",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Total Carrito DUNAROMA",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + "/pago-exitoso",
                CancelUrl = domain + "/carrito",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { id = session.Id, url = session.Url });
        }


        // ─── 2. EL NUEVO ENDPOINT (WEBHOOK) PARA ESCUCHAR A STRIPE ───
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = "whsec_b2dd36d6c7533e666855cf4136a3a1c1cbf1195b8794b214f1527bd28684811d"; // ¡Aquí pusimos la llave que te dio la consola!

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                  json,
                  Request.Headers["Stripe-Signature"],
                  endpointSecret
                );

                // Si el evento es de "Pago Completado"
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    // ¡AQUÍ LLEGÓ EL PAGO! En el futuro aquí guardarás la venta en tu base de datos.
                    System.Console.WriteLine($"\n\n💰 ¡ÉXITO! Se recibió un pago de Stripe para la sesión: {session.Id}\n\n");
                }

                // Le decimos a Stripe que lo recibimos bien (El famoso [200] OK)
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