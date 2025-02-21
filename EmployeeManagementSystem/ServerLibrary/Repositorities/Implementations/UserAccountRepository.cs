using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Context;
using ServerLibrary.Helpers;
using ServerLibrary.Repositorities.Contracts;
using SharedLibrary.DTOs;
using SharedLibrary.Entities;
using SharedLibrary.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServerLibrary.Repositorities.Implementations
{
    public class UserAccountRepository(IOptions<JwtSection> configuration, AppDBContext appDBContext) : IUserAccountRepository
    {
        public async Task<GeneralResponse> RegisterAsync(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                return new GeneralResponse(false, "Register model is null");
            }

            var checkUser = await FindUserByEmailAsync(registerDTO.Email);

            if (checkUser != null)
            {
                return new GeneralResponse(false, "User with this email already exists");
            }

            var appUser = await AddToDatabase(new AppUser
            {
                FullName = registerDTO.FullName,
                Email = registerDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password)
            });

            var checkAdminRole = await FindAdminRoleAsync();

            if (checkAdminRole == null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole { Name = Constants.AdminRole });
                await AddToDatabase(new UserRole()
                {
                    RoleId = createAdminRole.Id,
                    UserId = appUser.Id
                });
                return new GeneralResponse(true, "Admin user created successfully");
            }

            var checkUserRole = await FindUserRoleAsync();

            SystemRole response = new SystemRole();
            if (checkUserRole == null)
            {
                response = await AddToDatabase(new SystemRole { Name = Constants.UserRole });
                await AddToDatabase(new UserRole()
                {
                    RoleId = response.Id,
                    UserId = appUser.Id
                });
            }
            else
            {
                await AddToDatabase(new UserRole()
                {
                    RoleId = checkUserRole.Id,
                    UserId = appUser.Id
                });
            }

            return new GeneralResponse(true, "User created successfully");
        }
        public async Task<LoginResponse> LogInAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return new LoginResponse(false, "Login model is null");
            }

            var appUser = await FindUserByEmailAsync(loginDTO.Email);
            if (appUser == null)
            {
                return new LoginResponse(false, "User with this email does not exist");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, appUser.Password))
            {
                return new LoginResponse(false, "Email or Password is incorrect");
            }

            var getUserRole = await FindUserRole(appUser.Id);
            if (getUserRole == null)
            {
                return new LoginResponse(false, "User role not found");
            }

            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName == null)
            {
                return new LoginResponse(false, "User role not found");
            }

            string jwtToken = GenerateToken(appUser, getRoleName.Name);
            string refreshToken = GenerateRefreshToken();

            var findUser = await appDBContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == appUser.Id);
            if (findUser != null)
            {
                findUser.Token = refreshToken;
                await appDBContext.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshToken
                {
                    Token = refreshToken,
                    UserId = appUser.Id
                });

            }

            return new LoginResponse(true, "User logged in successfully", jwtToken, refreshToken);

        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO)
        {
            if (refreshTokenDTO == null)
            {
                return new LoginResponse(false, "Refresh token model is null");
            }

            var findToken = await appDBContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshTokenDTO.Token);
            if (findToken == null)
            {
                return new LoginResponse(false, "Refresh token is required");
            }

            var appUser = await appDBContext.AppUsers.FirstOrDefaultAsync(аu => аu.Id == findToken.UserId);
            if (appUser == null)
            {
                return new LoginResponse(false, "Refresh token can't be generated because user it's not found");
            }

            var getUserRole = await FindUserRole(appUser.Id);
            var getRoleName = await FindRoleName(getUserRole.RoleId);
            string jwtToken = GenerateToken(appUser, getRoleName.Name);
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await appDBContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == appUser.Id);
            if (updateRefreshToken == null)
            {
                return new LoginResponse(false, "Refresh token can't be generated because user has not signed in!");
            }

            updateRefreshToken.Token = refreshToken;
            await appDBContext.SaveChangesAsync();
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);

        }



        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string GenerateToken(AppUser appUser, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Value.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Name, appUser.FullName),
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: configuration.Value.Issuer,
                audience: configuration.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private async Task<UserRole> FindUserRole(int appUserId)
        {
            return await appDBContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == appUserId);
        }

        private async Task<SystemRole> FindRoleName(int roleId)
        {
            return await appDBContext.SystemRoles.FirstOrDefaultAsync(sr => sr.Id == roleId);
        }

        private async Task<AppUser> FindUserByEmailAsync(string email)
        {
            return await appDBContext.AppUsers.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
        }

        private async Task<SystemRole> FindAdminRoleAsync()
        {
            return await appDBContext.SystemRoles.FirstOrDefaultAsync(r => r.Name.Equals(Constants.AdminRole));
        }

        private async Task<SystemRole> FindUserRoleAsync()
        {
            return await appDBContext.SystemRoles.FirstOrDefaultAsync(r => r.Name.Equals(Constants.UserRole));
        }

        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = appDBContext.Add(model);
            await appDBContext.SaveChangesAsync();
            return (T)result.Entity;
        }


    }
}
