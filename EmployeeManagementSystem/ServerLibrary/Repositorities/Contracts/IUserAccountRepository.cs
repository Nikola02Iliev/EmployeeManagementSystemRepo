using SharedLibrary.DTOs;
using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Repositorities.Contracts
{
    public interface IUserAccountRepository
    {
        Task<GeneralResponse> RegisterAsync(RegisterDTO registerDTO);
        Task<LoginResponse> LogInAsync(LoginDTO loginDTO);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO);

    }
}
