using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Collections.Generic;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        [HttpPost("crear-sesion")]
        public IActionResult CrearSesion([FromBody] object carrito)
        {
            // Esta es la URL de tu Angular
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
                            UnitAmount = 92000, // Representa $920.00 (Stripe usa centavos)
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

            // Devolvemos el ID y la URL para que Angular nos redirija
            return Ok(new { id = session.Id, url = session.Url });
        }
    }
}