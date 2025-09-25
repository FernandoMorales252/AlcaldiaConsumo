using AlcaldiaFront.DTOs.CargoDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlcaldiaFront.Controllers
{
    public class CargoController : Controller
    {
        private readonly CargoService _cargoService;

        public CargoController(CargoService cargoService) => _cargoService = cargoService;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var cargo = await _cargoService.GetAllAsync();
                return View(cargo);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "No se pudieron listar los cargos: " + ex.Message;
                return View(new List<CargoRespuestaDTo>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var cargo = await _cargoService.GetByIdAsync(id);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoCrearDTo cargo)
        {
            if (!ModelState.IsValid)
            {
                return View(cargo);
            }
            try
            {
                var cargo2 = await _cargoService.CreateAsync(cargo, "tu_token_de_acceso");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el cargo: " + ex.Message);
                return View(cargo);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id_Cargo)
        {
            var cargo = await _cargoService.GetByIdAsync(Id_Cargo);
            if (cargo == null)
            {
                return NotFound();
            }

            var dto = new CargoActualizarDTo
            {
                Nombre_cargo = cargo.Nombre_cargo,
                Descripcion = cargo.Descripcion,
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id_Cargo, CargoActualizarDTo cargo)
        {

            if (!ModelState.IsValid)
            {
                return View(cargo);
            }
            var success = await _cargoService.UpdateAsync(Id_Cargo, cargo, "tu_token_de_acceso");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al actualizar el cargo.");
            return View(cargo);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id_Cargo)
        {
            var cargo = await _cargoService.GetByIdAsync(Id_Cargo);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id_Cargo)
        {
            var success = await _cargoService.DeleteAsync(Id_Cargo, "tu_token_de_acceso");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error al eliminar el cargo.");
            return View("Delete", await _cargoService.GetByIdAsync(Id_Cargo));
        }
    }
}
