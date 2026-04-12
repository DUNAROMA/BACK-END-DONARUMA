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
<<<<<<< HEAD
        public string Nombre { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen_Url { get; set; }
        public string Ocasion { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int Intensidad { get; set; }
        public int Dulzor { get; set; }
        public int Duracion { get; set; }
        public int Aromatico { get; set; }

        public List<int> FamiliasOlfativasIds { get; set; } = new List<int>();
=======
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Genero { get; set; }
        public string Ocasion { get; set; }
        public bool EsDeNoche { get; set; }
        public decimal Precio { get; set; }

        public string Descripcion { get; set; }
        public string Imagen_Url { get; set; }
        public int Stock { get; set; }
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
    }
}
