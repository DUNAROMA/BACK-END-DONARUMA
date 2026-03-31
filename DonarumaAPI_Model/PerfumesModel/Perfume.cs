using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Model.PerfumesModel
{
    public class Perfume
    {
<<<<<<< HEAD
        public int IdPerfume { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Ocasion { get; set; } = string.Empty;
     
        public string? FamiliaOlfativa { get; set; } 
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen_Url { get; set; }
        public int Stock { get; set; }

=======
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Marca { get; set; }
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen_Url { get; set; }
        public string? Ocasion { get; set; }
        public string? Genero { get; set; }
        public int? Stock { get; set; }
>>>>>>> origin/Eclud123
    }
}
