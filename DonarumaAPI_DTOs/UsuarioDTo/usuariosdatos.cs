using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerfumeApi.Models
{
    // Entidad principal que mapea a la tabla física
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("idusuario")]
        public long IdUsuario { get; set; }

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("apellidos")]
        public string? Apellidos { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }
    }

    // DTO para la función de búsqueda (Sin Llave)
    public class UsuarioDatosDto
    {
        [Column("nombre_completo")]
        public string? NombreCompleto { get; set; }
        [Column("direccion_usuario")]
        public string? DireccionUsuario { get; set; }
    }
}
