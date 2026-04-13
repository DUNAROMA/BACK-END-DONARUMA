using System;

namespace DonarumaAPI_DTOs.UsuarioDTo
{
    public class PerfilUsuarioDto
    {
        public string Nombre_Completo { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
    }

    public class EditarPerfilUsuarioDto
    {
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
    }

    public class CambiarContrasenaDto
    {
        public string ContrasenaActual { get; set; }
        public string NuevaContrasena { get; set; }
    }
}
