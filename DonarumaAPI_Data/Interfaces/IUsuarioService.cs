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
        Task<long> CrearUsuario(CrearUsuarioDTO usuario, string tokenConfirmacion);
        Task<UsuarioDTO> ObtenerUsuarioPorId(long idUsuario);

        Task<bool> ConfirmarCuenta(string token);
    }
}
