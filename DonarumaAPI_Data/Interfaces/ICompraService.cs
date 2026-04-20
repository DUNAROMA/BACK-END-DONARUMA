using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface ICompraService
    {
        Task<bool> ProcesarCompraExitosa(int idUsuario, string sessionId);
    }
}