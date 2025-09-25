using AlcaldiaFront.DTOs.TipoDocumentoDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;

    namespace AlcaldiaFront.Controllers
    {
        public class TipoDocController : Controller
        {
            private readonly TipoDocService _tipoDocService;

            public TipoDocController(TipoDocService tipoDocService)
            {
                _tipoDocService = tipoDocService;
            }

            public async Task<IActionResult> Index()
            {
                try
                {
                    var tipoDocs = await _tipoDocService.GetAllAsync();
                    return View(tipoDocs);
                }
                catch (Exception ex)
                {
                    
                    ViewBag.Error = "No se pudieron cargar los tipos de documentos: " + ex.Message;
                    return View(new List<TipoDocRespuestaDTO>());
                }
            }

     
            public async Task<IActionResult> Details(int id)
            {
                var tipoDoc = await _tipoDocService.GetByIdAsync(id);
                if (tipoDoc == null)
                {
                    return NotFound();
                }
                return View(tipoDoc);
            }

        
            public IActionResult Create()
            {
                return View();
            }

         
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(TipoDocCrearDTO tipoDocDto)
            {
                if (!ModelState.IsValid)
                {
                    return View(tipoDocDto);
                }

                try
                {
                    var nuevoTipoDoc = await _tipoDocService.CreateAsync(tipoDocDto, "tu_token_de_acceso");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el tipo de documento: " + ex.Message);
                    return View(tipoDocDto);
                }
            }

            
            public async Task<IActionResult> Edit(int id)
            {
                var tipoDoc = await _tipoDocService.GetByIdAsync(id);
                if (tipoDoc == null)
                {
                    return NotFound();
                }
            
                var dto = new TipoDocActualizarDTO
                {
                    Id_tipo = tipoDoc.Id_tipo,
                    Nombre = tipoDoc.Nombre
                };
                return View(dto);
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoDocActualizarDTO tipoDocDto)
        {
            // 2. Validación del modelo
            if (!ModelState.IsValid)
            {
                return View(tipoDocDto);
            }

            // 3. Llamada al servicio con el ID y el DTO
            var success = await _tipoDocService.UpdateAsync(id, tipoDocDto, "tu_token_de_acceso");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            // 4. Manejo de errores si la actualización falla
            ModelState.AddModelError("", "Error al actualizar el tipo de documento.");
            return View(tipoDocDto);
        }


        public async Task<IActionResult> Delete(int id)
            {
                var tipoDoc = await _tipoDocService.GetByIdAsync(id);
                if (tipoDoc == null)
                {
                    return NotFound();
                }
                return View(tipoDoc);
            }

           
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var success = await _tipoDocService.DeleteAsync(id, "tu_token_de_acceso");
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al eliminar el tipo de documento.");
                return View("Delete", await _tipoDocService.GetByIdAsync(id));
            }
        }
    }

