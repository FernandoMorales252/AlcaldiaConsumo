using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AlcaldiaFront.DTOs.ProyectoDTOs;

namespace AlcaldiaFront.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ProyectoService _proyectoService;
        private readonly MunicipioService _municipioService;

        public ProyectoController(ProyectoService proyectoService, MunicipioService municipioService)
        {
            _proyectoService = proyectoService;
            _municipioService = municipioService;
        }

        // GET: Proyecto
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

        // GET: Proyecto/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        // GET: Proyecto/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new ProyectoCrearDTo());
        }

        // POST: Proyecto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProyectoCrearDTo proyectoDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(proyectoDto);
            }

            try
            {
                var nuevoProyecto = await _proyectoService.CreateAsync(proyectoDto, "tu_token_de_acceso");
                if (nuevoProyecto == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el proyecto.");
                    await PopulateDropdowns();
                    return View(proyectoDto);
                }
                TempData["Ok"] = "Proyecto creado con éxito."; 
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el proyecto: " + ex.Message);
                await PopulateDropdowns();
                return View(proyectoDto);
            }
        }

        // GET: Proyecto/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }

            var dto = new ProyectoActualizarDTo
            {
                Id_Proyecto = proyecto.Id_Proyecto,
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                Fecha_Inicio = proyecto.Fecha_Inicio,
                Fecha_Fin = proyecto.Fecha_Fin,
                Presupuesto = proyecto.Presupuesto,
                Estado = proyecto.Estado,
                MunicipioId = proyecto.MunicipioId
            };
            await PopulateDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProyectoActualizarDTo proyectoDto)
        {
            if (id != proyectoDto.Id_Proyecto)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(proyectoDto);
            }

            var success = await _proyectoService.UpdateAsync(id, proyectoDto, "your_access_token");
            if (success)
            {
                TempData["Ok"] = "Proyecto actualizado con éxito."; 
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al actualizar el proyecto.");
            await PopulateDropdowns();
            return View(proyectoDto);

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
                    TempData["Ok"] = "Proyecto eliminado con éxito.";
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

        // Mapeo del Dropdown 
        private async Task PopulateDropdowns()
        {
            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");
        }
    }
}