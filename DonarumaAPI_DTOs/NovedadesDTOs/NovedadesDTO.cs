using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_DTOs.NovedadesDTOs
{
    public class NovedadesDTO
    {
        public int IdNovedad { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string ImagenUrl { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
