using AlcaldiaFront.DTOs.ProyectoDTOs;
using AlcaldiaFront.DTOs.QuejaDTOs;
using AlcaldiaFront.Models;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Globalization;


namespace AlcaldiaFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProyectoService _proyectoService;
        private readonly AvisoService _avisoService;
        private readonly QuejaService _quejaService;
        private readonly MunicipioService _municipioService;

        public HomeController(ILogger<HomeController> logger, ProyectoService proyectoService, AvisoService avisoService, QuejaService quejaService, MunicipioService municipioService)
        {
            _logger = logger;
            _proyectoService = proyectoService;
            _avisoService = avisoService;
            _quejaService = quejaService;
            _municipioService = municipioService;
        }

        public async Task<IActionResult> Index()
        {
           
            ViewData["PresupuestoTotalFormato"] = "$0,00";
            ViewData["PresupuestoEjecucionFormato"] = "$0,00";
            ViewData["ProyectosCulminados"] = 0;
            ViewData["AvisoReciente"] = null;

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

                var avisos = await _avisoService.GetAllAsync();

                if (avisos != null && avisos.Any())
                {
                    // Obtener el aviso más reciente basado en la Fecha_Registro
                    var avisoReciente = avisos
                        .OrderByDescending(a => a.Fecha_Registro)
                        .FirstOrDefault();

                    // Pasar el objeto completo del aviso a la vista
                    ViewData["AvisoReciente"] = avisoReciente;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos de proyectos o avisos para el Dashboard.");
              
            }



            return View();
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new QuejaCrearDTO());
        }

        // POST: Empleado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuejaCrearDTO quejaDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(quejaDto);
            }

            try
            {
                var nuevoQueja = await _quejaService.CreateAsync(quejaDto, "your_access_token");
                if (nuevoQueja == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el Reporte.");
                    await PopulateDropdowns();
                    return View(quejaDto);
                }
                TempData["Ok"] = "Reporte creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el Reporte: " + ex.Message);
                await PopulateDropdowns();
                return View(quejaDto);
            }
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

        public IActionResult History()
        {
            // La convención de ASP.NET Core buscará automáticamente
            // el archivo de vista: Views/Home/Historia.cshtml
            return View();
        }

        public IActionResult Contacto()
        {
            // La convención de ASP.NET Core buscará automáticamente
            // el archivo de vista: Views/Home/Historia.cshtml
            return View();
        }

        private async Task PopulateDropdowns()
        {
            // Carga y configura los dropdowns para Municipio y Cargo
            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");
        }
    }
}
