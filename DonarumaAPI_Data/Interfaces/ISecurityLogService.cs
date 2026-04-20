using DonarumaAPI_Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface ISecurityLogService
    {

        Task<IEnumerable<SecurityLog>> ObtenerLogsFiltrados(string? buscar, int limite);

        Task RegistrarEvento(string correoUsuario, string tipoEvento, string direccionIp);

    }
}