using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Model.CarritoModel
{
    public class CarritoItem
    {
        public int IdCarritoItem { get; set; } 

        public int IdUsuario { get; set; } 

        public int IdPerfume { get; set; } 

        public int Cantidad { get; set; }

        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;
    }
}