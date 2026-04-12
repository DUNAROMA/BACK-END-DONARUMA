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
<<<<<<< HEAD
        public string Nombre { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Ocasion { get; set; } = string.Empty;
        public string? FamiliaOlfativa { get; set; }
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen_Url { get; set; }
        public int Stock { get; set; }
    }
}
=======
        public string Nombre { get; set; }
        public string Marca { get; set; }

        // Estos son clave para las funciones de Alan:
        public string Genero { get; set; }     // Ej: "Hombre", "Mujer", "Unisex"
        public string Ocasion { get; set; }    // Ej: "Casual", "Trabajo", "Elegante"
        public bool EsDeNoche { get; set; }    // true si es de noche, false si es de día

        // Otros datos útiles
        public string FamiliaOlfativa { get; set; } // Ej: "Amaderado", "Cítrico"
        public decimal Precio { get; set; }
    }
}
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
