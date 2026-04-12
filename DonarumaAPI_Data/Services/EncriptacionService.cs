using DonarumaAPI_Data.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace DonarumaAPI_Data.Services
{
  
    
        public class EncriptacionService : IEncriptacionService
        {
            private readonly byte[] _key;
            private readonly byte[] _iv;

            public EncriptacionService(IConfiguration config)
            {
                // Guarda estas claves en appsettings.json o variables de entorno
                _key = Convert.FromBase64String(config["Encriptacion:Key"]);
                _iv = Convert.FromBase64String(config["Encriptacion:IV"]);
            }

            public string Encriptar(string texto)
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                var encriptador = aes.CreateEncryptor();
                var bytesTexto = Encoding.UTF8.GetBytes(texto);
                var bytesEncriptados = encriptador.TransformFinalBlock(bytesTexto, 0, bytesTexto.Length);

                return Convert.ToBase64String(bytesEncriptados);
            }

            public string Desencriptar(string textoCifrado)
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                var desencriptador = aes.CreateDecryptor();
                var bytesEncriptados = Convert.FromBase64String(textoCifrado);
                var bytesDesencriptados = desencriptador.TransformFinalBlock(bytesEncriptados, 0, bytesEncriptados.Length);

                return Encoding.UTF8.GetString(bytesDesencriptados);
            }
        }
}

