using DonarumaAPI_Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface ISecurityLogService
    {
        
        Task<IEnumerable<SecurityLog>> ObtenerTodosLosLogs();
    }
}