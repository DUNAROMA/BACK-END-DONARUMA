using DonarumaAPI_Data.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly string _apiKey;

        public ResendEmailService(IConfiguration config)
        {
            // Busca la API Key en Railway (Variables) o appsettings.json
            _apiKey = config["Resend:ApiKey"] ?? throw new ArgumentNullException("Resend ApiKey no configurada");
        }

        // --- FUNCIÓN 1: CONFIRMACIÓN DE CUENTA ---
        public async Task EnviarCorreoConfirmacion(string correoDestino, string token)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var body = new
            {
                from = "Dunaroma <soporte@donarumastore.com>",
                to = new[] { correoDestino },
                subject = "¡Bienvenido a Dunaroma! Confirma tu cuenta",
                html = $@"
                    <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h2>¡Hola! Gracias por unirte a Dunaroma.</h2>
                        <p>Tu código de seguridad es: <strong>{token}</strong></p>
                        <br>
                        <p>Para activar tu cuenta al instante, simplemente haz clic en el botón de abajo:</p>
                        <a href='https://donarumastore.com/#/verificar-codigo?token={token}' 
                           style='display: inline-block; padding: 12px 24px; background-color: #facc15; color: #111827; text-decoration: none; font-weight: bold; border-radius: 6px;'>
                           Verificar mi cuenta
                        </a>
                    </div>"
            };

            await EnviarPeticionResend(httpClient, request, body);
        }

        // --- FUNCIÓN 2: RECIBO DE COMPRA ---
        public async Task EnviarReciboCompra(string correoDestino, string nombreCliente, string numeroOrden, string detallesProductos)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var body = new
            {
                from = "Dunaroma <soporte@donarumastore.com>",
                to = new[] { correoDestino },
                subject = $"¡Gracias por tu compra en Dunaroma! (Orden #{numeroOrden})",
                html = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; color: #333;'>
                        <div style='background-color: #111827; padding: 20px; text-align: center;'>
                             <h1 style='color: #facc15; margin: 0;'>DUNAROMA</h1>
                        </div>
                        <h2 style='color: #111827;'>¡Hola {nombreCliente}!</h2>
                        <p>Hemos recibido tu pago con éxito y ya estamos preparando tu pedido.</p>
                        
                        <div style='background-color: #f3f4f6; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                            <h3 style='margin-top: 0;'>Resumen de tu orden (#{numeroOrden}):</h3>
                            <p style='white-space: pre-line;'>{detallesProductos}</p>
                        </div>
                        
                        <p>Te enviaremos otro correo en cuanto tu paquete esté en camino con el número de guía.</p>
                        <p>Si tienes alguna duda con tu pedido, responde directamente a este correo.</p>
                        <br>
                        <p>Atentamente,<br><strong>El equipo de Dunaroma</strong></p>
                    </div>"
            };

            await EnviarPeticionResend(httpClient, request, body);
        }

       
        private async Task EnviarPeticionResend(HttpClient client, HttpRequestMessage request, object body)
        {
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al enviar correo con Resend: {error}");
            }
        }
    }
}