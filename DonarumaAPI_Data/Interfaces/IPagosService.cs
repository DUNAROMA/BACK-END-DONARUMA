using DonarumaAPI_DTOs.Compras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IPagosService
    {
        Task<(string SessionId, string Url)> CrearSesionAsync(long idUsuario, CrearSesionDto dto);
        Task<object> ConfirmarPagoAsync(string sessionId);
    }
}

