using AlcaldiaFront.DTOs.ProyectoDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlcaldiaFront.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ProyectoService _proyectoService;

        public ProyectoController(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var proyectos = await _proyectoService.GetAllAsync();
                return View(proyectos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los proyectos: " + ex.Message;
                return View(new List<ProyectoRespuestaDTo>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProyectoCrearDTo proyectoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(proyectoDto);
            }

            try
            {
                var nuevoProyecto = await _proyectoService.CreateAsync(proyectoDto, "tu_token_de_acceso");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el proyecto: " + ex.Message);
                return View(proyectoDto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }

            var dto = new ProyectoActualizarDTo
            {
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                Fecha_Inicio = proyecto.Fecha_Inicio,
                Fecha_Fin = proyecto.Fecha_Fin,
                Presupuesto = proyecto.Presupuesto,
                Estado = proyecto.Estado,
                Id_Municipio = proyecto.Id_Municipio
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProyectoActualizarDTo proyectoDto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                return View(proyectoDto);
            }

            try
            {
                // Llamada al servicio con el ID y el DTO
                var success = await _proyectoService.UpdateAsync(id, proyectoDto, "tu_token_de_acceso");
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                // Manejo de errores si la actualización falla
                ModelState.AddModelError("", "Error al actualizar el proyecto.");
                return View(proyectoDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el proyecto: " + ex.Message);
                return View(proyectoDto);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _proyectoService.DeleteAsync(id, "tu_token_de_acceso");
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al eliminar el proyecto.");
                return View("Delete", await _proyectoService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el proyecto: " + ex.Message);
                return View("Delete", await _proyectoService.GetByIdAsync(id));
            }
        }
    }
}
