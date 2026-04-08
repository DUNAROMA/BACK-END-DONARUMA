using DonarumaAPI_DTOs.OfertasDTOs;
using DonarumaAPI_Model.OfertasModel;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IOfertasService
    {
        // Métodos para el Reloj
        Task<ConfiguracionReloj> ObtenerRelojActivoAsync();
        Task ActualizarRelojAsync(DateTime fechaFin);

        // Métodos para los Perfumes en Oferta
        Task<IEnumerable<OfertaResponseDTO>> ObtenerOfertasActivasAsync();
        Task<int> AgregarOModificarOfertaAsync(AgregarOfertaDTO ofertaDto);
        Task CambiarEstadoOfertaAsync(int idOferta, bool activo);

        Task<IEnumerable<VistaGestionOferta>> ObtenerOfertasDetalladasAsync();
    }
}