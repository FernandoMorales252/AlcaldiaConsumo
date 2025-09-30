using AlcaldiaFront.DTOs.EmpleadoDTOs;
using AlcaldiaFront.DTOs.InventarioDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlcaldiaFront.Controllers
{
    public class InventarioController : Controller
    {
        private readonly InventarioService _inventarioService;
        private readonly MunicipioService _municipioService;

        public InventarioController(InventarioService inventarioService, MunicipioService municipioService)
        {
            _inventarioService = inventarioService;
            _municipioService = municipioService;
        }

        // GET: Inventario
        public async Task<IActionResult> Index()
        {
            try
            {
                var inventarios = await _inventarioService.GetAllAsync();
                var municipios = await _municipioService.GetAllAsync();
                var municipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
                ViewBag.MunicipioNombres = municipioNombres;
                return View(inventarios);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los elementos: " + ex.Message;
                return View(new List<InventarioRespuestaDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var inventario = await _inventarioService.GetByIdAsync(id);
            var municipios = await _municipioService.GetAllAsync();
            var municipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            ViewBag.MunicipioNombres = municipioNombres;
            if (inventario == null)
            {
                return NotFound();
            }
            return View(inventario);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns(); 
            return View(new InventarioCrearDTO()); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventarioCrearDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(dto);
            }

            try
            {
                var created = await _inventarioService.CreateAsync(dto, "your_access_token");
                if (created == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el elemento de inventario.");
                    await PopulateDropdowns();
                    return View(dto);
                }
                TempData["Ok"] = "Elemento creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el elemento: " + ex.Message);
                await PopulateDropdowns();
                return View(dto);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var inventario = await _inventarioService.GetByIdAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }

            var dto = new InventarioActualizarDTO
            {
                Id_inventario = inventario.Id_inventario,
                Nombre_item = inventario.Nombre_item,
                Descripcion = inventario.Descripcion,
                Cantidad = inventario.Cantidad,
                Fecha_ingreso = inventario.Fecha_ingreso,
                Estado = inventario.Estado,
                MunicipioId = inventario.MunicipioId
            };
            await PopulateDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventarioActualizarDTO inventario)
        {

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(inventario);
            }
            var success = await _inventarioService.UpdateAsync(id, inventario, "");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al actualizar el elemento.");
            await PopulateDropdowns();
            return View(inventario);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var inventario = await _inventarioService.GetByIdAsync(id);
            var municipios = await _municipioService.GetAllAsync();
            var municipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            ViewBag.MunicipioNombres = municipioNombres;
            if (inventario == null)
            {
                return NotFound();
            }
            return View(inventario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _inventarioService.DeleteAsync(id, "tu_token_de_acceso");
            if (success)
            {
                TempData["Ok"] = "Elemento eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al eliminar el elemento de inventario.");
            return View("Delete", await _inventarioService.GetByIdAsync(id));
        }

        private async Task PopulateDropdowns()
        {
            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");
        }
    }
}




