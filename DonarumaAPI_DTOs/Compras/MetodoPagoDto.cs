using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DonarumaAPI_DTOs.Compras
{
    // Esta es la clase que representa la TABLA en PostgreSQL
    [Table("metodos_pago")]
    public class MetodoPago
    {
        [Key]
        [Column("id_tarjeta")] // Coincide con tu SQL
        public long IdTarjeta { get; set; }

        [Column("numero_tarjeta")] // Coincide con tu SQL
        public string? NumeroTarjeta { get; set; }

        [Column("nombre_titular")] // Coincide con tu SQL
        public string? NombreTitular { get; set; }
    }

    // Esta es la clase que usaremos para enviar DATOS a la API (DTO)
    public class MetodoPagoDto
    {
        public string? NumeroTarjeta { get; set; }
        public string? NombreTitular { get; set; }
    }
}