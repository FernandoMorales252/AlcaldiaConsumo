using AlcaldiaFront.DTOs.CargoDTOs;
using AlcaldiaFront.DTOs.DocumentoDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 


namespace AlcaldiaFront.Controllers
{
    public class DocumentoController : Controller
    {
        private readonly DocumentoService _documentoService;
        private readonly TipoDocService _tipoDocService;
        private readonly MunicipioService _municipioService;

        public DocumentoController(DocumentoService documentoService, TipoDocService tipoDocService, MunicipioService municipioService)
        {
            _documentoService = documentoService;
            _tipoDocService = tipoDocService;
            _municipioService = municipioService;
        }

        // GET: Documento
        // Displays a list of all documents.
        public async Task<IActionResult> Index()
        {
            try
            {
                var documentos = await _documentoService.GetAllAsync();
                var municipios = await _municipioService.GetAllAsync();
                var tipos = await _tipoDocService.GetAllAsync();
                var municipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
                ViewBag.MunicipioNombres = municipioNombres;
                var tipoNombres = tipos.ToDictionary(m => m.Id_tipo, m => m.Nombre);
                ViewBag.TipoNombres = tipoNombres;
                return View(documentos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los documentos: " + ex.Message;
                return View(new List<DocumentoRespuestaDTO>());
            }
        }

        // GET: Documento/Details/5
        // Displays the details of a single document.
        public async Task<IActionResult> Details(int id)
        {
            var documento = await _documentoService.GetByIdAsync(id);
            var municipios = await _municipioService.GetAllAsync();
            var tipos = await _tipoDocService.GetAllAsync();
            var municipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            ViewBag.MunicipioNombres = municipioNombres;
            var tipoNombres = tipos.ToDictionary(m => m.Id_tipo, m => m.Nombre);
            ViewBag.TipoNombres = tipoNombres;
            if (documento == null)
            {
                return NotFound();
            }
            return View(documento);
        }

        // GET: Documento/Create
        // Displays the form to create a new document.
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns(); // Populate the dropdown lists
            return View(new DocumentoCrearDTO()); // Pass an empty DTO to the view
        }

        // POST: Documento/Create
        // Processes the creation of a new document.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentoCrearDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(dto);
            }

            try
            {
                var created = await _documentoService.CreateAsync(dto, "your_access_token");
                if (created == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el documento.");
                    await PopulateDropdowns();
                    return View(dto);
                }
                TempData["Ok"] = "Documento creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el documento: " + ex.Message);
                await PopulateDropdowns();
                return View(dto);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var documento = await _documentoService.GetByIdAsync(id);
            if (documento == null)
            {
                return NotFound();
            }

            var dto = new DocumentoActualizarDTO
            {
                Id_documento = documento.Id_documento, // Usar documento.Id_documento
                Numero_documento = documento.Numero_documento, // Usar documento.Numero_documento
                Fecha_emision = documento.Fecha_emision,
                Propietario = documento.Propietario,
                Detalles = documento.Detalles,
                Estado = documento.Estado,
                TipoDocumentoId = documento.TipoDocumentoId,
                MunicipioId = documento.MunicipioId
            };
            await PopulateDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentoActualizarDTO documento)
        {

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(documento);
            }
            var success = await _documentoService.UpdateAsync(id, documento, "");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al actualizar el documento.");
            await PopulateDropdowns();
            return View(documento);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var documentos = await _documentoService.GetByIdAsync(id);
            var municipios = await _municipioService.GetAllAsync();
            var tipos = await _tipoDocService.GetAllAsync();
            var municipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            ViewBag.MunicipioNombres = municipioNombres;
            var tipoNombres = tipos.ToDictionary(m => m.Id_tipo, m => m.Nombre);
            ViewBag.TipoNombres = tipoNombres;
            if (documentos == null)
            {
                return NotFound();
            }
            return View(documentos);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _documentoService.DeleteAsync(id, "tu_token_de_acceso");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error al eliminar el cargo.");
            return View("Delete", await _documentoService.GetByIdAsync(id));
        }

        // This is a helper method to fetch data for dropdowns.
        private async Task PopulateDropdowns()
        {
            var tipoDocs = await _tipoDocService.GetAllAsync();
            ViewBag.TipoDocumentoId = new SelectList(tipoDocs, "Id_tipo", "Nombre");

            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");
        }
    }
}
