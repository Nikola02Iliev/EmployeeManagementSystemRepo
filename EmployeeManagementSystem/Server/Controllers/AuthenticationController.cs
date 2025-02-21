using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositorities.Contracts;
using SharedLibrary.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUserAccountRepository userAccountRepository) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO registerDTO)
        {
            if(registerDTO == null)
            {
                return BadRequest("Model is empty!");
            }

            var result = await userAccountRepository.RegisterAsync(registerDTO);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return BadRequest("Model is empty!");
            }

            var result = await userAccountRepository.LogInAsync(loginDTO);

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO)
        {
            if (refreshTokenDTO == null)
            {
                return BadRequest("Model is empty!");
            }

            var result = await userAccountRepository.RefreshTokenAsync(refreshTokenDTO);
            return Ok(result);
        }
    }
}
