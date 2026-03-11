using DonarumaAPI_Model.PerfumesModel;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IPerfumeService
    {
        Task<IEnumerable<Perfume>> ObtenerPorMarcaAsync(string marca);
        Task<IEnumerable<Perfume>> BuscarPorNombreAsync(string nombre);
        Task<IEnumerable<Perfume>> ObtenerPorPrecioMayorAsync();
        Task<IEnumerable<Perfume>> ObtenerPorPrecioMenorAsync();
    }
}