using AlcaldiaFront.DTOs.ProyectoDTOs;
using AlcaldiaFront.Models;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace AlcaldiaFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProyectoService _proyectoService;

        public HomeController(ILogger<HomeController> logger , ProyectoService proyectoService)
        {
            
            _logger = logger;
            _proyectoService = proyectoService;
        }

        public async Task<IActionResult> Index()
        {
           
            ViewData["PresupuestoTotalFormato"] = "$0,00";
            ViewData["PresupuestoEjecucionFormato"] = "$0,00";
            ViewData["ProyectosCulminados"] = 0;

            try
            {
                
                var proyectos = await _proyectoService.GetAllAsync();

                if (proyectos != null)
                {
                    var culture = CultureInfo.GetCultureInfo("es-ES");

                    decimal presupuestoTotal = proyectos.Sum(p => p.Presupuesto);
                    ViewData["PresupuestoTotalFormato"] = presupuestoTotal.ToString("C", culture);

                    int proyectosCulminados = proyectos.Count(p => p.Estado.Equals("finalizado", StringComparison.OrdinalIgnoreCase));
                    ViewData["ProyectosCulminados"] = proyectosCulminados;

                    decimal presupuestoEjecucion = proyectos
                        .Where(p => p.Estado.Equals("en_ejecucion", StringComparison.OrdinalIgnoreCase))
                        .Sum(p => p.Presupuesto);
                    ViewData["PresupuestoEjecucionFormato"] = presupuestoEjecucion.ToString("C", culture);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos de proyectos para el Dashboard.");
              
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
