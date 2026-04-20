using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DonarumaAPI_DTOs.Compras
{
    
    [Table("metodos_pago")]
    public class MetodoPago
    {
        [Key]
        [Column("id_tarjeta")] 
        public long IdTarjeta { get; set; }

        [Column("numero_tarjeta")] 
        public string? NumeroTarjeta { get; set; }

        [Column("nombre_titular")] 
        public string? NombreTitular { get; set; }
    }

    
    public class MetodoPagoDto
    {
        public string? NumeroTarjeta { get; set; }
        public string? NombreTitular { get; set; }
    }
}