using DonarumaAPI_DTOs.Compras;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IMetodosPagoService
    {
        Task<IEnumerable<MetodoPagoDto>> GetMetodosPagoUsuarioAsync(long idUsuario);
        Task<string> EliminarMetodoPagoAsync(long idUsuario, long idTarjeta);
    }
}