using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Model.PerfumesModel
{
    public class Perfume
    {
        public int IdPerfume { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Genero { get; set; }     // Ej: "Hombre", "Mujer", "Unisex"
        public string Ocasion { get; set; }    // Ej: "Casual", "Trabajo", "Elegante"
        public bool EsDeNoche { get; set; }    // true si es de noche, false si es de día
        public string FamiliaOlfativa { get; set; } // Ej: "Amaderado", "Cítrico"
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen_Url { get; set; }
        public int? Stock { get; set; }
    }
}
