using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonarumaAPI_DTOs.Compras
{
    [Table("perfumes")]
    public class Perfume
    {
        [Key]
        [Column("idperfume")]
        public long IdPerfume { get; set; }

        [Column("nombreperfume")]
        public string? Nombre { get; set; }

        [Column("marca")]
        public string? Marca { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("imagen_url")]
        public string? ImagenUrl { get; set; }

        [Column("ocasion")]
        public string? Ocasion { get; set; }

        [Column("genero")]
        public string? Genero { get; set; }

        [Column("stock")]
        public int Stock { get; set; }
    }
}