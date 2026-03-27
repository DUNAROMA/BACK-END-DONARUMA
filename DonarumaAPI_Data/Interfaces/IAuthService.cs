using DonarumaAPI_DTOs.LoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginDTO login, CancellationToken ct = default);
        Task<LoginResponse> Refresh(string refreshToken, CancellationToken ct = default);
        Task Logout(int idUsuario, CancellationToken ct = default);
    }
}
