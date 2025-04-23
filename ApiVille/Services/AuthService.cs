using ApiVille.DTOs;
using ApiVille.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiVille.Services
{
    public class AuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;

        public AuthService(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterDto model)
        {
            var user = new AppUser
            {
                Nome = model.Nome,
                Cognome = model.Cognome,
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage, object? TokenData)> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return (false, "Credenziali non valide", null);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return (true, null, new
            {
                token,
                ruolo = roles.FirstOrDefault(),
                username = user.UserName,
                email = user.Email
            });
        }
    }
}
