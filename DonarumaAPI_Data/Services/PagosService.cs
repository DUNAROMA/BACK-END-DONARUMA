// Services/PagosService.cs
using Dapper;
using DonarumaAPI_Data;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using DonarumaAPI_Model.UsuariosModel;
using Npgsql;
using Stripe;
using Stripe.Checkout;
using System.Data;

namespace Backend_Donaruma.Services
{
    public class PagosService : IPagosService
    {
        private readonly string _connectionString;
        private readonly IEncriptacionService _encriptacion;

        public PagosService(PostgreSQLConfiguration config, IEncriptacionService encriptacion)
        {
            _connectionString = config.ConnectionString;
            _encriptacion = encriptacion;
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        // ─────────────────────────────────────────────────────────
        // CREAR SESIÓN
        // ─────────────────────────────────────────────────────────
        public async Task<(string SessionId, string Url)> CrearSesionAsync(long idUsuario, CrearSesionDto dto)
        {
            // 1. Obtener usuario de la BD
            using var db = Connection;

            var usuario = await db.QueryFirstOrDefaultAsync<Usuarios>(
                @"SELECT idusuario, nombre, correo, ""StripeCustomerId""
                  FROM usuarios
                  WHERE idusuario = @idusuario",
                new { idusuario = idUsuario }
            );

            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            // 2. Verificar o crear Customer en Stripe
            string stripeCustomerId;
            var customerService = new CustomerService();

            if (string.IsNullOrEmpty(usuario.StripeCustomerId))
            {
                var nuevoCustomer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Email = usuario.Correo,
                    Name = usuario.Nombre,
                    Metadata = new Dictionary<string, string>
                    {
                        { "idusuario", idUsuario.ToString() }
                    }
                });

                stripeCustomerId = nuevoCustomer.Id;

                await db.ExecuteAsync(
                    @"UPDATE usuarios
                      SET ""StripeCustomerId"" = @stripecustomerid
                      WHERE idusuario = @idusuario",
                    new { stripecustomerid = stripeCustomerId, idusuario = idUsuario }
                );
            }
            else
            {
                stripeCustomerId = usuario.StripeCustomerId;
            }

            // 3. Crear sesión de Stripe con Metadata del carrito
            var sessionOptions = new SessionCreateOptions
            {
                Customer = stripeCustomerId,

                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    SetupFutureUsage = "on_session"
                },

                PaymentMethodTypes = new List<string> { "card" },

                Metadata = new Dictionary<string, string>
                {
                    { "idusuario",  idUsuario.ToString() },
                    { "direccion",  dto.Direccion ?? "" },
                    { "perfumes",   string.Join(",", dto.PerfumesIds) },
                    { "cantidades", string.Join(",", dto.Cantidades) }
                },

                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount  = 92000,
                            Currency    = "mxn",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Total Carrito DUNAROMA"
                            }
                        },
                        Quantity = 1
                    }
                },

                Mode = "payment",
                SuccessUrl = "http://localhost:4200/pago-exitoso?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:4200/carrito",
            };

            var session = await new SessionService().CreateAsync(sessionOptions);

            return (session.Id, session.Url);
        }

        // ─────────────────────────────────────────────────────────
        // CONFIRMAR PAGO
        // ─────────────────────────────────────────────────────────
        public async Task<object> ConfirmarPagoAsync(string sessionId)
        {
            // 1. Verificar con Stripe que el pago fue exitoso
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(sessionId, new SessionGetOptions
            {
                Expand = new List<string> { "payment_intent.payment_method" }
            });

            if (session.PaymentStatus != "paid")
                throw new Exception("El pago no fue completado.");

            Console.WriteLine($"[CONFIRMAR] Sesión verificada: {session.Id} — Status: {session.PaymentStatus}");

            // 2. Leer datos del carrito desde Metadata
            var metadata = session.Metadata;
            var idUsuario = long.Parse(metadata["idusuario"]);
            var direccion = metadata["direccion"];
            var perfumes = metadata["perfumes"].Split(',').Select(long.Parse).ToArray();
            var cantidades = metadata["cantidades"].Split(',').Select(int.Parse).ToArray();

            Console.WriteLine($"[CONFIRMAR] IdUsuario: {idUsuario} — Perfumes: {metadata["perfumes"]}");

            using var db = Connection;

            // 3. Verificar que esta sesión no fue procesada ya
            var yaExiste = await db.ExecuteScalarAsync<bool>(
                @"SELECT EXISTS(
                    SELECT 1 FROM compra WHERE stripe_session_id = @sessionId
                  )",
                new { sessionId }
            );

            if (yaExiste)
            {
                Console.WriteLine("[CONFIRMAR] Compra ya registrada, se omite duplicado.");
                return new { mensaje = "Compra ya registrada." };
            }

            // 4. Guardar método de pago en MetodosDePago
            var paymentIntent = session.PaymentIntent;
            var paymentMethod = paymentIntent.PaymentMethod;
            var card = paymentMethod.Card;
            var paymentMethodId = paymentMethod.Id;

            var vencimiento = $"{card.ExpMonth:D2}/{card.ExpYear}";
            var vencimientoEncriptado = _encriptacion.Encriptar(vencimiento);

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

            Console.WriteLine("[CONFIRMAR] ✅ Método de pago guardado.");

            // 5. Obtener IdMetodo recién guardado
            var idMetodo = await db.ExecuteScalarAsync<int?>(
                @"SELECT ""IdMetodo"" FROM ""MetodosDePago""
                  WHERE ""IdUsuario"" = @idusuario
                  ORDER BY ""FechaRegistro"" DESC
                  LIMIT 1",
                new { idusuario = idUsuario }
            );

            // 6. Registrar la compra en BD
            await db.ExecuteScalarAsync<string>(
                @"SELECT procesar_compra_con_stock(
                    @p_idusuario,
                    @p_direccion,
                    @p_idmetodo,
                    @p_perfumes_ids,
                    @p_cantidades,
                    @p_session_id
                  )",
                new
                {
                    p_idusuario = idUsuario,
                    p_direccion = direccion,
                    p_idmetodo = idMetodo,
                    p_perfumes_ids = perfumes,
                    p_cantidades = cantidades,
                    p_session_id = sessionId
                }
            );

            Console.WriteLine("[CONFIRMAR] ✅ Compra registrada en BD.");

            return new { mensaje = "Compra registrada exitosamente." };
        }
    }
}