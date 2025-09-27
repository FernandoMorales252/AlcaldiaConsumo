using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AlcaldiaFront.DTOs.EmpleadoDTOs;

namespace AlcaldiaFront.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly EmpleadoService _empleadoService;
        private readonly MunicipioService _municipioService;
        private readonly CargoService _cargoService;

        public EmpleadoController(EmpleadoService empleadoService, MunicipioService municipioService, CargoService cargoService)
        {
            _empleadoService = empleadoService;
            _municipioService = municipioService;
            _cargoService = cargoService;
        }

        // GET: Empleado
        public async Task<IActionResult> Index()
        {
            try
            {
                var empleados = await _empleadoService.GetAllAsync("your_access_token");

                // Obtener listas para los nombres de los municipios y cargos
                var municipios = await _municipioService.GetAllAsync();
                var cargos = await _cargoService.GetAllAsync();

                // Convertir las listas a diccionarios para la vista
                ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
                ViewBag.CargoNombres = cargos.ToDictionary(c => c.Id_Cargo, c => c.Nombre_cargo);

                return View(empleados);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los empleados: " + ex.Message;
                // Si hay un error, inicializar los ViewBag para evitar NullReferenceException
                ViewBag.MunicipioNombres = new Dictionary<int, string>();
                ViewBag.CargoNombres = new Dictionary<int, string>();
                return View(new List<EmpleadoRespuestaDTo>());
            }
        }

        // GET: Empleado/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var empleado = await _empleadoService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();
            var cargos = await _cargoService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            ViewBag.CargoNombres = cargos.ToDictionary(c => c.Id_Cargo, c => c.Nombre_cargo);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        // GET: Empleado/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new EmpleadoCrearDTo());
        }

        // POST: Empleado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoCrearDTo empleadoDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(empleadoDto);
            }

            try
            {
                var nuevoEmpleado = await _empleadoService.CreateAsync(empleadoDto, "your_access_token");
                if (nuevoEmpleado == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el empleado.");
                    await PopulateDropdowns();
                    return View(empleadoDto);
                }
                TempData["Ok"] = "Empleado creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el empleado: " + ex.Message);
                await PopulateDropdowns();
                return View(empleadoDto);
            }
        }

        // GET: Empleado/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var empleado = await _empleadoService.GetByIdAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            var dto = new EmpleadoActualizarDTo
            {
                Id_empleado = empleado.Id_empleado,
                Nombre = empleado.Nombre,
                Apellido = empleado.Apellido,
                Fecha_contratacion = empleado.Fecha_contratacion,
                Estado = empleado.Estado,
                CargoId = empleado.CargoId,
                MunicipioId = empleado.MunicipioId
            };
            await PopulateDropdowns();
            return View(dto);
        }

        // POST: Empleado/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmpleadoActualizarDTo empleadoDto)
        {
            if (id != empleadoDto.Id_empleado)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(empleadoDto);
            }
            try
            {
                var success = await _empleadoService.UpdateAsync(id, empleadoDto, "your_access_token");
                if (success)
                {
                    TempData["Ok"] = "Empleado actualizado con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al actualizar el empleado.");
                await PopulateDropdowns();
                return View(empleadoDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el empleado: " + ex.Message);
                await PopulateDropdowns();
                return View(empleadoDto);

            }
        }

        // GET: Empleado/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var empleado = await _empleadoService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();
            var cargos = await _cargoService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            ViewBag.CargoNombres = cargos.ToDictionary(c => c.Id_Cargo, c => c.Nombre_cargo);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        // POST: Empleado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _empleadoService.DeleteAsync(id, "your_access_token");
                if (success)
                {
                    TempData["Ok"] = "Empleado eliminado con éxito.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al eliminar el empleado.");
                return View("Delete", await _empleadoService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el empleado: " + ex.Message);
                return View("Delete", await _empleadoService.GetByIdAsync(id));
            }
        }

        // Mapeo del Dropdown 
        private async Task PopulateDropdowns()
        {
            // Carga y configura los dropdowns para Municipio y Cargo
            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");

            var cargos = await _cargoService.GetAllAsync();
            ViewBag.CargoId = new SelectList(cargos, "Id_Cargo", "Nombre_cargo");
        }
    }
}