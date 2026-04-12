using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// DonarumaAPI_DTOs/Compras/CrearSesionDto.cs
namespace DonarumaAPI_DTOs.Compras
{
    public class CrearSesionDto
    {
        public string Direccion { get; set; }
        public long[] PerfumesIds { get; set; }
        public int[] Cantidades { get; set; }
    }
}
