using AlcaldiaFront.Services;
using AlcaldiaFront.WebApp.DTOs.UsuarioDTOs;
using AlcaldiaFront.WebApp.Helpers;
using AlcaldiaFront.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace AlcaldiaFront.WebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApiService _apiService;

        public UsuarioController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Listar usuarios
        public async Task<IActionResult> Index()
        {
            var token = AuthHelper.ObtenerToken(User); // Obtener token desde Claims
            var usuarios = await _apiService.GetAllAsync<UsuarioDTO>("User/usuarios", token);
            return View(usuarios);
        }
    }
}