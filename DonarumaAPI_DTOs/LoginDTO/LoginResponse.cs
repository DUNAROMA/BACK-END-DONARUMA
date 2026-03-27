using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_DTOs.LoginDTO
{
    // RefreshDTO.cs
    public sealed record RefreshRequestDTO(string RefreshToken);

    // Actualiza LoginResponse.cs
    public sealed class LoginResponse
    {
        public bool Exito { get; init; }
        public string Mensaje { get; init; } = string.Empty;
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
    }
}
