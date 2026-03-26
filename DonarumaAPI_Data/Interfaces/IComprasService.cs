using DonarumaAPI_DTOs.Compras;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IComprasService
    {
        Task<string> ProcesarCompraAsync(ProcesarCompraDto request);
    }
}