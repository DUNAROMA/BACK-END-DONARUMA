using DonarumaAPI_DTOs.FamiliaOlfativa;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IFamiliaOlfativaService
    {
        Task<IEnumerable<FamiliaOlfativaDTO>> GetTodosLosPerfumesAsync();
    }
}