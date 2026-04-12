using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_DTOs.OfertasDTOs
{
    public class VistaGestionOferta
    {
        public int idoferta { get; set; }
        public int idperfume { get; set; }
        public decimal descuento { get; set; }
        public decimal preciooferta { get; set; }
        public bool activo { get; set; }
        public DateTime fechacreacion { get; set; }
        public string nombre_perfume { get; set; } = string.Empty;
        public string marca { get; set; } = string.Empty;
        public decimal precio_original { get; set; }
        public string imagen_url { get; set; } = string.Empty;
        public string? nombre_url 
        {
            get;
        }
    }
}