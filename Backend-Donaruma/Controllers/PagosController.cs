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
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options); 

            return Ok(new { id = session.Id, url = session.Url });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = "salomon dividiste al bebe ;--)";

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
                        System.Console.WriteLine($"\n\n💰 ¡ÉXITO! Se recibió un pago de Stripe para la sesión: {session.Id}\n\n");
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