// Controllers/StripeWebhookController.cs
using Backend_Donaruma.Services;
using Dapper;
using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Stripe;
using Stripe.Checkout;
using System.Data;

namespace Backend_Donaruma.Controllers
{
    [Route("api/stripe/webhook")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _webhookSecret;
        private readonly IEncriptacionService _encriptacion;

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public StripeWebhookController(PostgreSQLConfiguration config, IConfiguration appConfig, IEncriptacionService encriptacion)
        {
            _connectionString = config.ConnectionString;
            _webhookSecret = appConfig["Stripe:WebhookSecret"]!;
            _encriptacion = encriptacion;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, signature, _webhookSecret);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WEBHOOK] Error validando firma: {ex.Message}");
                return BadRequest("Webhook inválido.");
            }

            Console.WriteLine($"[WEBHOOK] Evento recibido: {stripeEvent.Type}");

            // ─────────────────────────────────────────
            // EVENTO 1: Registrar compra en BD
            // ─────────────────────────────────────────
            if (stripeEvent.Type == "checkout.session.completed")
            {
                try
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session?.Metadata == null)
                    {
                        Console.WriteLine("[WEBHOOK] checkout.session.completed: Metadata vacío.");
                        return Ok();
                    }

                    Console.WriteLine($"[WEBHOOK] Metadata recibido: {string.Join(", ", session.Metadata.Select(m => $"{m.Key}={m.Value}"))}");

                    var idUsuario = long.Parse(session.Metadata["idusuario"]);
                    var direccion = session.Metadata["direccion"];
                    var perfumes = session.Metadata["perfumes"].Split(',').Select(long.Parse).ToArray();
                    var cantidades = session.Metadata["cantidades"].Split(',').Select(int.Parse).ToArray();

                    using var db = Connection;

                    var idMetodo = await db.ExecuteScalarAsync<int?>(
                        @"SELECT ""IdMetodo"" FROM ""MetodosDePago""
                  WHERE ""IdUsuario"" = @idusuario
                  ORDER BY ""FechaRegistro"" DESC
                  LIMIT 1",
                        new { idusuario = idUsuario }
                    );

                    Console.WriteLine($"[WEBHOOK] IdMetodo encontrado: {idMetodo?.ToString() ?? "NULL"}");

                    await db.ExecuteScalarAsync<string>(
                        @"SELECT procesar_compra_con_stock(
                    @p_idusuario,
                    @p_direccion,
                    @p_idmetodo,
                    @p_perfumes_ids,
                    @p_cantidades
                  )",
                        new
                        {
                            p_idusuario = idUsuario,
                            p_direccion = direccion,
                            p_idmetodo = idMetodo,
                            p_perfumes_ids = perfumes,
                            p_cantidades = cantidades
                        }
                    );

                    Console.WriteLine("[WEBHOOK] ✅ Compra registrada exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WEBHOOK] ❌ Error en checkout.session.completed: {ex.Message}");
                    return Ok(); // Siempre retorna Ok a Stripe aunque haya error interno
                }
            }

            // ─────────────────────────────────────────
            // EVENTO 2: Guardar método de pago en BD
            // ─────────────────────────────────────────
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                try
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var paymentMethodId = paymentIntent?.PaymentMethodId;

                    if (paymentMethodId == null)
                    {
                        Console.WriteLine("[WEBHOOK] payment_intent.succeeded: PaymentMethodId nulo.");
                        return Ok();
                    }

                    Console.WriteLine($"[WEBHOOK] PaymentMethodId: {paymentMethodId}");

                    var pmService = new PaymentMethodService();
                    var paymentMethod = await pmService.GetAsync(paymentMethodId);
                    var card = paymentMethod.Card;

                    var customerService = new CustomerService();
                    var customer = await customerService.GetAsync(paymentIntent.CustomerId);
                    var idUsuario = long.Parse(customer.Metadata["idusuario"]);

                    var vencimiento = $"{card.ExpMonth:D2}/{card.ExpYear}";
                    var vencimientoEncriptado = _encriptacion.Encriptar(vencimiento);

                    using var db = Connection;
                    await db.ExecuteAsync(
                        @"INSERT INTO ""MetodosDePago""
                    (""IdUsuario"", ""StripePaymentMethodId"", ""Ultimos4"", ""Marca"", ""VencimientoEncriptado"")
                  VALUES
                    (@idusuario, @stripepaymentmethodid, @ultimos4, @marca, @vencimientoencriptado)
                  ON CONFLICT (""StripePaymentMethodId"") DO NOTHING",
                        new
                        {
                            idusuario = idUsuario,
                            stripepaymentmethodid = paymentMethodId,
                            ultimos4 = card.Last4,
                            marca = card.Brand,
                            vencimientoencriptado = vencimientoEncriptado
                        }
                    );

                    Console.WriteLine("[WEBHOOK] ✅ Método de pago guardado exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WEBHOOK] ❌ Error en payment_intent.succeeded: {ex.Message}");
                    return Ok();
                }
            }

            return Ok();
        }
    }
}