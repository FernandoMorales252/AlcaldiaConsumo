using AlcaldiaFront.DTOs.EmpleadoDTOs;
using AlcaldiaFront.DTOs.InventarioDTOs;
using AlcaldiaFront.Services;
using AlcaldiaFront.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlcaldiaFront.Controllers
{
    [Authorize]
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

        // GET: Inventario/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            // Usamos el ViewModel en lugar del DTO
            return View(new InventarioCrearVM());
        }

        // POST: Inventario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Recibe el ViewModel que incluye IFormFile
        public async Task<IActionResult> Create(InventarioCrearVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(viewModel);
            }

            // 1. CONVERSIÓN: Si hay archivo, convertirlo a Base64 y asignarlo al campo del DTO
            if (viewModel.ArchivoImagen != null)
            {
                viewModel.ImagenBase64 = await ConvertIFormFileToBase64(viewModel.ArchivoImagen);
            }

            try
            {
                // 2. LLAMADA AL SERVICIO: El ViewModel (que hereda del DTO) se envía directamente
                var created = await _inventarioService.CreateAsync(viewModel, "your_access_token");

                // ... (resto de la lógica) ...

                TempData["Ok"] = "Elemento creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el elemento: " + ex.Message);
                await PopulateDropdowns();
                return View(viewModel);
            }
        }


        // GET: Inventario/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var inventario = await _inventarioService.GetByIdAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }

            // 1. Mapeo: Mapeamos los campos de la respuesta (DTO) al ViewModel
            var viewModel = new InventarioActualizarVM
            {
                // Campos del DTO de Actualización
                Nombre_item = inventario.Nombre_item,
                Descripcion = inventario.Descripcion,
                Cantidad = inventario.Cantidad,
                Fecha_ingreso = inventario.Fecha_ingreso,
                Estado = inventario.Estado,
                MunicipioId = inventario.MunicipioId,
                // Campo del ViewModel para mostrar la imagen actual
                ImagenDataExistente = inventario.Imagen_data_base64 // Este es el Base64 que viene de la API
            };

            await PopulateDropdowns();
            return View(viewModel);
        }

        // POST: Inventario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Recibe el ViewModel que incluye el IFormFile (si se subió uno nuevo)
        public async Task<IActionResult> Edit(int id, InventarioActualizarVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(viewModel);
            }

            // 1. CONVERSIÓN: Si se subió un NUEVO archivo, convertirlo a Base64.
            if (viewModel.ArchivoImagen != null)
            {
                // Asignamos la nueva imagen Base64 al campo del DTO
                viewModel.ImagenBase64 = await ConvertIFormFileToBase64(viewModel.ArchivoImagen);
            }
            // NOTA: Si viewModel.ArchivoImagen es null, viewModel.ImagenBase64 será null (el valor por defecto).
            // Esto es correcto, ya que en el servicio de la API implementamos la lógica:
            // Si ImagenBase64 es null, se mantiene la imagen existente en la BD.

            // 2. LLAMADA AL SERVICIO
            // Enviamos el ViewModel (mapeado al DTO)
            var success = await _inventarioService.UpdateAsync(id, viewModel, "tu_token_de_acceso");

            if (success)
            {
                TempData["Ok"] = "Elemento actualizado con éxito.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error al actualizar el elemento.");
            await PopulateDropdowns();
            return View(viewModel);
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

        // --- Método Auxiliar para Conversión de Archivo ---
        private async Task<string?> ConvertIFormFileToBase64(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            // Puedes agregar aquí validaciones de tamaño o tipo (MIME) si es necesario.

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            // Convertir el arreglo de bytes a la cadena Base64
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}




