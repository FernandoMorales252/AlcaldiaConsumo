using AlcaldiaFront.DTOs.AvisoDTOs;
using AlcaldiaFront.DTOs.DocumentoDTOs;
using AlcaldiaFront.DTOs.EmpleadoDTOs;
using AlcaldiaFront.DTOs.QuejaDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlcaldiaFront.Controllers
{
    [Authorize]
    public class AvisoController : Controller
    {
        private readonly AvisoService _avisoService;
        private readonly MunicipioService _municipioService;

        public AvisoController(AvisoService avisoService, MunicipioService municipioService)
        {
            _avisoService = avisoService;
            _municipioService = municipioService;
        }

        // GET: Empleado
        public async Task<IActionResult> Index()
        {
            try
            {
                var avisos = await _avisoService.GetAllAsync("your_access_token");

                // Obtener listas para los nombres de los municipios y cargos
                var municipios = await _municipioService.GetAllAsync();

                // Convertir las listas a diccionarios para la vista
                ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);

                return View(avisos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los avisos: " + ex.Message;
                // Si hay un error, inicializar los ViewBag para evitar NullReferenceException
                ViewBag.MunicipioNombres = new Dictionary<int, string>();
                return View(new List<AvisoRespuestaDTO>());
            }
        }

        public async Task<IActionResult> Menu()
        {
            try
            {
                var avisos = await _avisoService.GetAllAsync("your_access_token");

                // Obtener listas para los nombres de los municipios y cargos
                var municipios = await _municipioService.GetAllAsync();

                // Convertir las listas a diccionarios para la vista
                ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);

                return View(avisos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los avisos: " + ex.Message;
                // Si hay un error, inicializar los ViewBag para evitar NullReferenceException
                ViewBag.MunicipioNombres = new Dictionary<int, string>();
                return View(new List<AvisoRespuestaDTO>());
            }
        }

        // GET: Empleado/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var aviso = await _avisoService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            if (aviso == null)
            {
                return NotFound();
            }
            return View(aviso);
        }

        public async Task<IActionResult> Public(int id)
        {
            var aviso = await _avisoService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            if (aviso == null)
            {
                return NotFound();
            }
            return View(aviso);
        }

        // GET: Empleado/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new AvisoCrearDTO());
        }

        // POST: Empleado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AvisoCrearDTO avisoDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(avisoDto);
            }

            try
            {
                var nuevoAviso = await _avisoService.CreateAsync(avisoDto, "your_access_token");
                if (nuevoAviso == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el aviso.");
                    await PopulateDropdowns();
                    return View(avisoDto);
                }
                TempData["Ok"] = "Aviso creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el aviso: " + ex.Message);
                await PopulateDropdowns();
                return View(avisoDto);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var aviso = await _avisoService.GetByIdAsync(id);
            if (aviso == null)
            {
                return NotFound();
            }

            var dto = new AvisoActualizarDTO
            {
                Id_aviso = aviso.Id_aviso,
                Titulo = aviso.Titulo,
                Descripcion = aviso.Descripcion,
                Fecha_Registro = aviso.Fecha_Registro,
                Tipo = aviso.Tipo,
                MunicipioId = aviso.MunicipioId
            };
            await PopulateDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AvisoActualizarDTO aviso)
        {

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(aviso);
            }
            var success = await _avisoService.UpdateAsync(id, aviso, "");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al actualizar el aviso.");
            await PopulateDropdowns();
            return View(aviso);
        }


        // GET: Empleado/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var aviso = await _avisoService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            if (aviso == null)
            {
                return NotFound();
            }
            return View(aviso);
        }


        // POST: Empleado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _avisoService.DeleteAsync(id, "your_access_token");
                if (success)
                {
                    TempData["Ok"] = "Aviso eliminado con éxito.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al eliminar el aviso.");
                return View("Delete", await _avisoService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el aviso: " + ex.Message);
                return View("Delete", await _avisoService.GetByIdAsync(id));
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
