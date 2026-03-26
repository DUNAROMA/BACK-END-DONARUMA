using DonarumaAPI_DTOs.Compras;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IPerfumesStockService
    {
        Task<IEnumerable<Perfume>> GetPerfumesDisponiblesAsync();
    }
}