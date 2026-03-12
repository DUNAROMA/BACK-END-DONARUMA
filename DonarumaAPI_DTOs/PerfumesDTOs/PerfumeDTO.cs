using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_DTOs.PerfumesDTOs
{
    public class PerfumeDTO
    {
        public int IdPerfume { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Genero { get; set; }
        public string Ocasion { get; set; }
        public bool EsDeNoche { get; set; }
        public decimal Precio { get; set; }

        public string Descripcion { get; set; }
        public string Imagen_Url { get; set; }
        public int Stock { get; set; }
    }
}
