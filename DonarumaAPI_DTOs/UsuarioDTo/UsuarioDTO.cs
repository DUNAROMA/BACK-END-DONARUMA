using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_DTOs.UsuarioDTo
{
    public class UsuarioDTO
    {
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Direccion { get; set; }
        public string? Correo { get; set; }
    }
    public class ActualizarUsuarioDTO
    {
        public long IdUsuario { get; set; } 
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Direccion { get; set; }
    }

    public class CambiarPasswordDTO
    {
        public long IdUsuario { get; set; }
        public string ContrasenaActual { get; set; }
        public string ContrasenaNueva { get; set; }
    }
}