using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_DTOs.CarritoDTOs
{
    public class AgregarAlCarritoDTO
    {
        public int IdUsuario { get; set; }
        public int IdPerfume { get; set; }
        public int Cantidad { get; set; }
    }
}
