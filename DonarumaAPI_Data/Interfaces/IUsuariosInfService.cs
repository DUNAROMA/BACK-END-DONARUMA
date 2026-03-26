using DonarumaAPI_DTOs.Compras;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IUsuariosInfService
    {
        Task<UsuarioDatosDto?> GetDatosUsuarioAsync(long idUsuario);
    }
}