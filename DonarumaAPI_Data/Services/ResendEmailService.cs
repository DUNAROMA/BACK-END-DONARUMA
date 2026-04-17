using DonarumaAPI_Data.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DonarumaAPI_Data.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly string _apiKey;

        public ResendEmailService(IConfiguration config)
        {
            // Buscamos la llave de Resend en el appsettings o en las variables de Railway
            _apiKey = config["Resend:ApiKey"]!;
        }

        public async Task EnviarCorreoConfirmacion(string correoDestino, string token)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");

            // Le ponemos nuestra llave secreta de Resend como gafete de entrada
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // Armamos el correo
            var body = new
            {
                from =  "Dunaroma <soporte@donarumastore.com>", // 👈 Obligatorio usar este mientras probamos
                to = new[] { correoDestino },
                subject = "¡Bienvenido a Dunaroma! Confirma tu cuenta",
                html = $"<h2>¡Hola! Gracias por unirte a Dunaroma.</h2><p>Por favor confirma tu cuenta. Tu código de seguridad es: <strong>{token}</strong></p>"
            };

            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Enviamos el misil
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al enviar correo con Resend: {error}");
            }
        }
    }
}