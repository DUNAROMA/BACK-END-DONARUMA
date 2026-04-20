using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly ICompraService _compraService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmailService _emailService;

        // 👇 1. DECLARAMOS LA PLUMA DE SEGURIDAD AQUÍ
        private readonly ISecurityLogService _securityLogService;

        // 👇 2. LA PEDIMOS EN EL CONSTRUCTOR
        public PagosController(
            IPerfumeService perfumeService,
            ICompraService compraService,
            IUsuarioService usuarioService,
            IEmailService emailService,
            ISecurityLogService securityLogService) // <--- Agregada aquí
        {
            _perfumeService = perfumeService;
            _compraService = compraService;
            _usuarioService = usuarioService;
            _emailService = emailService;

            // 👇 3. LA INICIALIZAMOS
            _securityLogService = securityLogService;
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
                    { "Items", string.Join(", ", request.items.Select(i => $"{i.IdPerfume}x{i.cantidad}")) },
                    { "ResumenPedido", string.Join("\n", request.items.Select(i => $"• {i.cantidad}x {i.nombre}")) }
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
                            // 1. Guardamos la compra y vaciamos el carrito
                            bool exito = await _compraService.ProcesarCompraExitosa(idUsuario, session.Id);

                            if (exito)
                            {
                                // 2. Buscamos al cliente en la base de datos
                                var usuario = await _usuarioService.ObtenerUsuarioPorId(idUsuario);

                                // 3. Sacamos el resumen bonito que guardamos en la Metadata
                                session.Metadata.TryGetValue("ResumenPedido", out string? resumen);

                                // Creamos un número de orden corto
                                string numeroOrden = session.Id.Substring(session.Id.Length - 8).ToUpper();

                                if (usuario != null)
                                {
                                    // 4. Disparamos correo
                                    await _emailService.EnviarReciboCompra(
                                        correoDestino: usuario.Correo ?? "",
                                        nombreCliente: usuario.Nombre ?? "Cliente",
                                        numeroOrden: numeroOrden,
                                        detallesProductos: resumen ?? "Productos en tu carrito"
                                    );

                                    // 👇 5. REGISTRAMOS EL EVENTO EN LA AUDITORÍA (SÓLO EL DE LA COMPRA)
                                    string ipCliente = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Stripe Webhook";
                                    await _securityLogService.RegistrarEvento(usuario.Correo ?? "Sistema", $"Compra Exitosa - Orden #{numeroOrden}", ipCliente);

                                    System.Console.WriteLine($"\n\n✉️ ¡Correo de recibo enviado a {usuario.Correo} y log registrado!\n\n");
                                }
                            }
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