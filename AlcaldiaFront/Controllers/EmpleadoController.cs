using AlcaldiaFront.DTOs.EmpleadoDTOs;
using AlcaldiaFront.Services;
using AlcaldiaFront.WebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlcaldiaFront.Controllers
{
    [Authorize]
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

        private string ObtenerToken()
        {
            // Se asume que AuthHelper.ObtenerToken(User) extrae el JWT de las Claims de la cookie "AuthCookie"
            return AuthHelper.ObtenerToken(User);
        }

        // GET: Empleado
        public async Task<IActionResult> Index()
        {
            try
            {
                // CORRECTO: Usamos el token real
                var empleados = await _empleadoService.GetAllAsync(ObtenerToken());

                // Obtener listas para los nombres de los municipios y cargos
                var municipios = await _municipioService.GetAllAsync();
                var cargos = await _cargoService.GetAllAsync();

                // Convertir las listas a diccionarios para la vista
                ViewBag.MunicipioNombres = municipios?.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio) ?? new Dictionary<int, string>();
                ViewBag.CargoNombres = cargos?.ToDictionary(c => c.Id_Cargo, c => c.Nombre_cargo) ?? new Dictionary<int, string>();

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
            // CORRECTO: Usamos el token real
            var empleado = await _empleadoService.GetByIdAsync(id, ObtenerToken());

            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();
            var cargos = await _cargoService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios?.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio) ?? new Dictionary<int, string>();
            ViewBag.CargoNombres = cargos?.ToDictionary(c => c.Id_Cargo, c => c.Nombre_cargo) ?? new Dictionary<int, string>();
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
                // CORRECTO: Usamos el token real
                var nuevoEmpleado = await _empleadoService.CreateAsync(empleadoDto, ObtenerToken());
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


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // CORRECTO: Usamos el token real
            var empleado = await _empleadoService.GetByIdAsync(id, ObtenerToken());
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmpleadoActualizarDTo empleado)
        {

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(empleado);
            }

            // CORRECTO: Usamos el token real
            var success = await _empleadoService.UpdateAsync(id, empleado, ObtenerToken());

            if (success)
            {
                TempData["Ok"] = "Empleado actualizado con éxito.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error al actualizar el empleado. Verifique si el ID coincide o si la sesión es válida.");
            await PopulateDropdowns();
            return View(empleado);
        }

        // GET: Empleado/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // CORRECTO: Usamos el token real
            var empleado = await _empleadoService.GetByIdAsync(id, ObtenerToken());
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();
            var cargos = await _cargoService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios?.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio) ?? new Dictionary<int, string>();
            ViewBag.CargoNombres = cargos?.ToDictionary(c => c.Id_Cargo, c => c.Nombre_cargo) ?? new Dictionary<int, string>();
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
                // CORRECTO: Usamos el token real
                var success = await _empleadoService.DeleteAsync(id, ObtenerToken());
                if (success)
                {
                    TempData["Ok"] = "Empleado eliminado con éxito.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al eliminar el empleado.");
                return View("Delete", await _empleadoService.GetByIdAsync(id, ObtenerToken()));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el empleado: " + ex.Message);
                return View("Delete", await _empleadoService.GetByIdAsync(id, ObtenerToken()));
            }
        }


        [HttpGet]
        // Esta acción es la que descarga el archivo.
        public async Task<IActionResult> Exportar()
        {
            // 1. OBTENER EL TOKEN REAL: Usamos el método centralizado.
            string token = ObtenerToken();

            if (string.IsNullOrEmpty(token))
            {
                // Si el token no está, redireccionamos al login o mostramos un error de autenticación.
                TempData["Error"] = "Error de autenticación. No se encontró el token en la sesión.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // 2. Llama al servicio para obtener el array de bytes
                var fileBytes = await _empleadoService.ExportarExcelAsync(token);

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    // Si el servicio devuelve null o vacío, es porque la API falló (401, 403, 404, o error interno).
                    TempData["Error"] = "No se pudo obtener el archivo. Revisa la consola para ver el error HTTP de la API.";
                    return RedirectToAction(nameof(Index));
                }

                string excelName = $"Empleados_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

         
                return File(
                    fileContents: fileBytes,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: excelName
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al intentar descargar el Excel: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Mapeo del Dropdown 
        private async Task PopulateDropdowns()
        {
            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");

            var cargos = await _cargoService.GetAllAsync();
            ViewBag.CargoId = new SelectList(cargos, "Id_Cargo", "Nombre_cargo");
        }
    }
}