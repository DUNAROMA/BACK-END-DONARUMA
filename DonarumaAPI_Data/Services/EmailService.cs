using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Mail;

namespace DonarumaAPI_Data.Services
{
    public interface IEmailService
    {
        Task EnviarCorreoConfirmacion(string correoDestino, string token);
    }

    public class GmailEmailService : IEmailService
    {
        public async Task EnviarCorreoConfirmacion(string correoDestino, string token)
        {
            var email = new MimeMessage();

            // 1. Quién lo envía (Pon el correo real de tu tienda)
            email.From.Add(new MailboxAddress("Dunaroma Store", "TU_CORREO_DE_TIENDA@gmail.com"));

            // 2. A quién se lo enviamos
            email.To.Add(new MailboxAddress("", correoDestino));

            // 3. Asunto
            email.Subject = "¡Bienvenido! Confirma tu cuenta en Dunaroma";

            // ⚠️ Ajustamos el link para usar tu localhost (Angular) por ahora
            string linkValidacion = $"http://localhost:4200/verificar?token={token}";

            // 4. El diseño del correo (Puedes usar HTML para que se vea bonito)
            email.Body = new TextPart("html")
            {
                Text = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; text-align: center;'>
                        <h2>¡Hola! Gracias por registrarte en Dunaroma.</h2>
                        <p>Para poder iniciar sesión y empezar a comprar, necesitamos verificar tu correo electrónico.</p>
                        <a href='{linkValidacion}' style='background-color: #fbbf24; color: #111827; padding: 10px 20px; text-decoration: none; font-weight: bold; border-radius: 5px; display: inline-block; margin-top: 15px;'>Confirmar mi cuenta</a>
                    </div>"
            };

            // 5. El envío real
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            // Conectamos a los servidores de Google
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            // 🔒 Pon tu correo y la Contraseña de Aplicación de 16 letras (NO tu contraseña normal)
            await smtp.AuthenticateAsync("dunaroma4@gmail.com", "hutklhaliqgxamyg");

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}