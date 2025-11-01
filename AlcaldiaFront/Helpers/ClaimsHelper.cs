using AlcaldiaFront.WebApp.DTOs.UsuarioDTOs;
using System.Security.Claims;

namespace AlcaldiaFront.WebApp.Helpers
{
    public static class ClaimsHelper
    {
        public static ClaimsPrincipal CrearClaimsPrincipal(LoginResponseDTO usuario)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol),
            new Claim("Token", usuario.Token)
        };

            var identity = new ClaimsIdentity(claims, "AuthCookie");
            return new ClaimsPrincipal(identity);
        }
    }
}
