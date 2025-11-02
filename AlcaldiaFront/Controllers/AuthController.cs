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
            // Se asume que el servicio LoginAsync devolverá null si las credenciales son incorrectas
            // (ya sea correo o contraseña).
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                // Mensaje más claro para el usuario.
                ViewBag.Error = "Correo electrónico o contraseña incorrectos.";
                return View(dto); // Devolver el DTO para mantener los datos del formulario.
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
            try
            {
                // Se asume que RegistrarAsync puede lanzar una excepción específica
                // (p. ej., DuplicateEmailException) o devolver un resultado que indica el error.
                var result = await _authService.RegistrarAsync(dto);

                if (result == null || result.Id <= 0)
                {
                    // Si result es null o tiene Id <= 0, asumimos un error general o no capturado.
                    ViewBag.Error = "Error al registrar. Por favor, intente de nuevo.";
                    return View(dto);
                }

                // Crear y firmar los claims usando el helper
                var principal = ClaimsHelper.CrearClaimsPrincipal(result);

                await HttpContext.SignInAsync("AuthCookie", principal);

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex) // Se recomienda usar una excepción más específica (como DuplicateEmailException)
            {
                // Lógica para detectar si el error fue por duplicidad de email.
                // Esto depende de cómo implementes el error en tu capa de servicio.
                if (ex.Message.Contains("duplicate email", System.StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("correo en uso", System.StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Error = "Este correo electrónico ya está en uso. Intente iniciar sesión.";
                }
                else
                {
                    // Error genérico si no es por duplicidad de email
                    ViewBag.Error = "Error al registrar. Por favor, intente de nuevo.";
                }

                return View(dto);
            }
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