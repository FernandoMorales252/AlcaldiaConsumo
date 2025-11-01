using AlcaldiaFront.DTOs.ProyectoDTOs;
using AlcaldiaFront.DTOs.QuejaDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlcaldiaFront.Controllers
{
    [Authorize]
    public class QuejaController : Controller
    {
        private readonly QuejaService _quejaService;
        private readonly MunicipioService _municipioService;

        public QuejaController(QuejaService quejaService, MunicipioService municipioService)
        {
            _quejaService = quejaService;
            _municipioService = municipioService;
        }

        // GET: Empleado
        public async Task<IActionResult> Index()
        {
            try
            {
                var avisos = await _quejaService.GetAllAsync("your_access_token");

                // Obtener listas para los nombres de los municipios y cargos
                var municipios = await _municipioService.GetAllAsync();

                // Convertir las listas a diccionarios para la vista
                ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);

                return View(avisos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar las quejas: " + ex.Message;
                // Si hay un error, inicializar los ViewBag para evitar NullReferenceException
                ViewBag.MunicipioNombres = new Dictionary<int, string>();
                return View(new List<QuejaRespuestaDTO>());
            }
        }

        public async Task<IActionResult> Menu()
        {
            try
            {
                var avisos = await _quejaService.GetAllAsync("your_access_token");

                // Obtener listas para los nombres de los municipios y cargos
                var municipios = await _municipioService.GetAllAsync();

                // Convertir las listas a diccionarios para la vista
                ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);

                return View(avisos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar las quejas: " + ex.Message;
                // Si hay un error, inicializar los ViewBag para evitar NullReferenceException
                ViewBag.MunicipioNombres = new Dictionary<int, string>();
                return View(new List<QuejaRespuestaDTO>());
            }
        }


        // GET: Empleado/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var queja = await _quejaService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            if (queja == null)
            {
                return NotFound();
            }
            return View(queja);
        }

        // GET: Empleado/Create


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var queja = await _quejaService.GetByIdAsync(id);
            if (queja == null)
            {
                return NotFound();
            }

            var dto = new QuejaActualizarDTO
            {
                Id_queja = queja.Id_queja,
                Titulo = queja.Titulo,
                Descripcion = queja.Descripcion,
                Fecha_Registro = queja.Fecha_Registro,
                Tipo = queja.Tipo,
                Nivel = queja.Nivel,
                MunicipioId = queja.MunicipioId
            };
            await PopulateDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuejaActualizarDTO queja)
        {

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(queja);
            }
            var success = await _quejaService.UpdateAsync(id, queja, "");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al actualizar el Reporte.");
            await PopulateDropdowns();
            return View(queja);
        }

        // GET: Empleado/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var queja = await _quejaService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            if (queja == null)
            {
                return NotFound();
            }
            return View(queja);
        }

        // POST: Empleado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _quejaService.DeleteAsync(id, "your_access_token");
                if (success)
                {
                    TempData["Ok"] = "Reporte eliminado con éxito.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al eliminar el aviso.");
                return View("Delete", await _quejaService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el aviso: " + ex.Message);
                return View("Delete", await _quejaService.GetByIdAsync(id));
            }
        }

        // Mapeo del Dropdown 
        private async Task PopulateDropdowns()
        {
            // Carga y configura los dropdowns para Municipio y Cargo
            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");
        }
    }
}
