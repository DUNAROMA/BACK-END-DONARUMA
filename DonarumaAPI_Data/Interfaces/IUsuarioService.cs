using DonarumaAPI_DTOs.UsuarioDTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IUsuarioService
    {
        Task<long> CrearUsuario(CrearUsuarioDTO usuario);
        Task<UsuarioDTO> ObtenerUsuarioPorId(long idUsuario);
        Task AgregarDireccion(long idUsuario, DireccionDTO dto); // NUEVO
        Task<PerfilUsuarioDto> ObtenerPerfilUsuarioAsync(long idUsuario);
        Task EditarPerfilUsuarioAsync(long idUsuario, EditarPerfilUsuarioDto dto);
        Task<bool> CambiarContrasenaUsuarioAsync(long idUsuario, string contrasenaActual, string nuevaContrasenaHash);
    }
}
