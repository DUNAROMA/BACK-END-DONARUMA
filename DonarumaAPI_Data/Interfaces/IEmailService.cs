using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreoConfirmacion(string correoDestino, string token);
        Task EnviarReciboCompra(string correoDestino, string nombreCliente, string numeroOrden, string detallesProductos);
    }
}
