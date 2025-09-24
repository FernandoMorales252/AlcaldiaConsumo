using AlcaldiaFront.DTOs.MunicipioDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlcaldiaFront.Controllers
{
    public class MunicipioController : Controller
    {
        private readonly MunicipioService _municipioService;

        public MunicipioController(MunicipioService municipioService)
        {
            _municipioService = municipioService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var municipios = await _municipioService.GetAllAsync();
                return View(municipios);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los municipios: " + ex.Message;
                return View(new List<MunicipioRespuestaDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var municipio = await _municipioService.GetByIdAsync(id);
            if (municipio == null)
            {
                return NotFound();
            }
            return View(municipio);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MunicipioCrearDTO municipioDto)
        {
            if (!ModelState.IsValid)
            {
                return View(municipioDto);
            }
            try
            {
                var nuevoMunicipio = await _municipioService.CreateAsync(municipioDto, "tu_token_de_acceso");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el municipio: " + ex.Message);
                return View(municipioDto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var municipio = await _municipioService.GetByIdAsync(id);
            if (municipio == null)
            {
                return NotFound();
            }

            var dto = new MunicipioActualizarDTO
            {
                Nombre_Municipio = municipio.Nombre_Municipio
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MunicipioActualizarDTO municipioDto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                return View(municipioDto);
            }

            // Llamada al servicio con el ID y el DTO
            var success = await _municipioService.UpdateAsync(id, municipioDto, "tu_token_de_acceso");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            // Manejo de errores si la actualización falla
            ModelState.AddModelError("", "Error al actualizar el municipio.");
            return View(municipioDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var municipio = await _municipioService.GetByIdAsync(id);
            if (municipio == null)
            {
                return NotFound();
            }
            return View(municipio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _municipioService.DeleteAsync(id, "tu_token_de_acceso");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error al eliminar el municipio.");
            return View("Delete", await _municipioService.GetByIdAsync(id));
        }
    }
}
