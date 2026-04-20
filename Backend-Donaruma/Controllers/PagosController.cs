using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DonarumaAPI_Data.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Backend_Donaruma.Controllers
{

    public class ItemCarrito
    {
        public int IdPerfume { get; set; }
        public int cantidad { get; set; }


        public string nombre { get; set; } = string.Empty;
        public decimal precio { get; set; }
    }

    public class CheckoutRequest
    {
        public int IdUsuario { get; set; }
        public List<ItemCarrito> items { get; set; } = new List<ItemCarrito>();
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly IPerfumeService _perfumeService;


        public PagosController(IPerfumeService perfumeService)
        {
            _perfumeService = perfumeService;
        }

        [Authorize]
        [HttpPost("crear-sesion")]
        public async Task<IActionResult> CrearSesion([FromBody] CheckoutRequest request)
        {
            var domain = "https://www.donarumastore.com";
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in request.items)
            {

                var perfumeReal = await _perfumeService.ObtenerPerfumePorId(item.IdPerfume);

                if (perfumeReal == null)
                {
                    return BadRequest(new { mensaje = $"El producto con ID {item.IdPerfume} no existe o fue eliminado." });
                }

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {

                        UnitAmount = (long)(perfumeReal.Precio * 100),
                        Currency = "mxn",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {

                            Name = perfumeReal.Nombre,
                        },
                    },

                    Quantity = item.cantidad > 0 ? item.cantidad : 1,
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = domain + "/pago-exitoso",
                CancelUrl = domain + "/carrito",

                Metadata = new Dictionary<string, string>
                {
                    { "IdUsuario", request.IdUsuario.ToString() },
                    { "Items", string.Join(", ", request.items.Select(i => $"{i.IdPerfume}x{i.cantidad}")) }
                }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new { id = session.Id, url = session.Url });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

           
            var endpointSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");

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

                    if (session != null)
                    {
                        
                        if (session.Metadata.TryGetValue("IdUsuario", out string? idUsuarioString) && int.TryParse(idUsuarioString, out int idUsuario))
                        {
                            System.Console.WriteLine($"\n\n💰 ¡ÉXITO! El usuario con ID {idUsuario} acaba de pagar la sesión {session.Id}\n\n");

                           
                            
                        }
                        else
                        {
                            System.Console.WriteLine("\n\n⚠️ Se recibió un pago, pero la sesión no tenía un IdUsuario asociado.\n\n");
                        }
                    }
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