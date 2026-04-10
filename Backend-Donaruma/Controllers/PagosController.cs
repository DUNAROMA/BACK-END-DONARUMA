using Microsoft.AspNetCore.Mvc;
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
    public class PagosController : ControllerBase
    {
        // ─── 2. EL ENDPOINT ACTUALIZADO (PRECIOS DINÁMICOS) ───
        [HttpPost("crear-sesion")]
        public IActionResult CrearSesion([FromBody] CheckoutRequest request)
        {
            var domain = "http://localhost:4200";

            // Armamos la lista de productos dinámicamente
            var lineItems = new List<SessionLineItemOptions>();

            // Recorremos cada perfume que Angular nos mandó en el carrito
            foreach (var item in request.items)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = item.precio * 100, // Stripe siempre cobra en centavos, multiplicamos x 100
                        Currency = "mxn",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.nombre, // ¡Aparecerá el nombre real del perfume en Stripe!
                        },
                    },
                    Quantity = item.cantidad,
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems, // Pasamos la lista dinámica aquí
                Mode = "payment",
                SuccessUrl = domain + "/pago-exitoso",
                CancelUrl = domain + "/carrito",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { id = session.Id, url = session.Url });
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