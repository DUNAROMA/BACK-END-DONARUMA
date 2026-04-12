using DonarumaAPI_DTOs.Compras;
using DonarumaAPI_DTOs.Compras.DonarumaAPI_DTOs.Compras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface ICompraService
    {
        Task<CompraResultadoDto> ProcesarCompraAsync(ProcesarCompraDto request);
    }
}
