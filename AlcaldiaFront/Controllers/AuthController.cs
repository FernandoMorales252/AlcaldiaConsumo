using AlcaldiaFront.WebApp.DTOs.UsuarioDTOs;
using AlcaldiaFront.WebApp.Helpers;
using AlcaldiaFront.WebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FrontendRestauranteMarisco.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: Mostrar Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                ViewBag.Error = "Credenciales inválidas";
                return View();
            }

            // Crear y firmar los claims usando el helper
            var principal = ClaimsHelper.CrearClaimsPrincipal(result);

            await HttpContext.SignInAsync("AuthCookie", principal);

            return RedirectToAction("Index", "Home");
        }
        // POST: Registro
        [HttpPost]
        public async Task<IActionResult> Registrar(UsuarioRegistroDTO dto)
        {
            var result = await _authService.RegistrarAsync(dto);
            if (result == null || result.Id <= 0)
            {
                ViewBag.Error = "Error al registrar";
                return View();
            }

            // Crear y firmar los claims usando el helper
            var principal = ClaimsHelper.CrearClaimsPrincipal(result);

            await HttpContext.SignInAsync("AuthCookie", principal);

            return RedirectToAction("Index", "Home");
        }
        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie");
            return RedirectToAction("Login");
        }

        // GET: Mostrar Registro
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }
    }
}