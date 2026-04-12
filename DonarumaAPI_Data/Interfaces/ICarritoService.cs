using DonarumaAPI_DTOs.CarritoDTOs;
using DonarumaAPI_Model.CarritoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface ICarritoService
    {
        Task<bool> AgregarAlCarrito(AgregarAlCarritoDTO item);
        Task<IEnumerable<dynamic>> ObtenerCarritoPorUsuario(int idUsuario);
        Task<bool> EliminarItemCarrito(int idCarritoItem);
        Task<bool> VaciarCarrito(int idUsuario);
    }
}
