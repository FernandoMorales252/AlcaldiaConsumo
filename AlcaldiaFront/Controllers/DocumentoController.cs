using AlcaldiaFront.DTOs.CargoDTOs;
using AlcaldiaFront.DTOs.DocumentoDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 


namespace AlcaldiaFront.Controllers
{
    [Authorize]
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


        // Dentro de la clase DocumentoController
        public async Task<IActionResult> Index(int? MunicipioId, int? TipoDocumentoId, string Estado, string NombrePropietario)
        {
            try
            {
                var documentos = await _documentoService.GetAllAsync();

                // Centraliza la carga de datos auxiliares para filtros y nombres
                await PopulateFilterDropdowns();

                if (documentos != null && documentos.Any())
                {
                    var documentosFiltrados = documentos.AsQueryable();

                    // Lógica de filtrado (como se detalló en el punto 1)
                    // ... [Insertar aquí la lógica de filtrado] ... 
                    if (MunicipioId.HasValue && MunicipioId.Value > 0)
                    {
                        documentosFiltrados = documentosFiltrados.Where(d => d.MunicipioId == MunicipioId.Value);
                    }

                    if (TipoDocumentoId.HasValue && TipoDocumentoId.Value > 0)
                    {
                        documentosFiltrados = documentosFiltrados.Where(d => d.TipoDocumentoId == TipoDocumentoId.Value);
                    }

                    if (!string.IsNullOrEmpty(Estado) && Estado != "Todos")
                    {
                        documentosFiltrados = documentosFiltrados.Where(d => d.Estado.Equals(Estado, StringComparison.OrdinalIgnoreCase));
                    }

                    if (!string.IsNullOrEmpty(NombrePropietario))
                    {
                        documentosFiltrados = documentosFiltrados.Where(d =>
                            d.Propietario != null &&
                            d.Propietario.Contains(NombrePropietario, StringComparison.OrdinalIgnoreCase));
                    }

                    // Pasar los valores de filtro actuales (IMPORTANTE)
                    ViewBag.CurrentMunicipioId = MunicipioId;
                    ViewBag.CurrentTipoDocumentoId = TipoDocumentoId;
                    ViewBag.CurrentEstado = Estado;
                    ViewBag.CurrentNombrePropietario = NombrePropietario;

                    return View(documentosFiltrados.ToList());
                }

                return View(new List<DocumentoRespuestaDTO>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los documentos: " + ex.Message;
                await PopulateFilterDropdowns(); // Asegurar que los ViewBag estén llenos incluso con error
                return View(new List<DocumentoRespuestaDTO>());
            }
        }

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

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns(); 
            return View(new DocumentoCrearDTO()); 
        }

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
                ModelState.AddModelError("", "Error al crear: Asegurate de que el numero no este en uso y llenar los campos");
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
                Id_documento = documento.Id_documento, 
                Numero_documento = documento.Numero_documento,
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
            ModelState.AddModelError("", "Error al actualizar: Asegurate de que el numero no este en uso y llenar los campos.");
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

        // Dentro de la clase DocumentoController
        private async Task PopulateFilterDropdowns()
        {
            // Cargar listas para filtros
            var municipios = await _municipioService.GetAllAsync();
            var tipos = await _tipoDocService.GetAllAsync();

            // 1. Dropdowns para la sección de Filtro (con opción "Todos")
            // Municipio
            var municipiosList = municipios.Select(m => new SelectListItem
            {
                Value = m.Id_Municipio.ToString(),
                Text = m.Nombre_Municipio
            }).ToList();
            municipiosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Municipios" });
            ViewBag.MunicipioId = municipiosList;

            // Tipo Documento
            var tiposList = tipos.Select(t => new SelectListItem
            {
                Value = t.Id_tipo.ToString(),
                Text = t.Nombre
            }).ToList();
            tiposList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Tipos" });
            ViewBag.TipoDocumentoId = tiposList;

            // Estado (Estatica, se puede mejorar usando un enum o lista compartida)
            var estadosList = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Todos los Estados" },
        new SelectListItem { Value = "Vigente", Text = "Activo" },
        new SelectListItem { Value = "Anulado", Text = "Inactivo" },
        new SelectListItem { Value = "Revision", Text = "Revisión" }
        // Añade aquí todos los estados posibles
    };
            ViewBag.EstadoList = estadosList;


            // 2. Diccionarios de Nombres (para mostrar en la tabla de resultados)
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            ViewBag.TipoNombres = tipos.ToDictionary(m => m.Id_tipo, m => m.Nombre);
        }

        // Nota: La función existente 'PopulateDropdowns' la puedes mantener para Create/Edit, 
        // ya que no necesita la opción "Todos".
        private async Task PopulateDropdowns()
        {
            var tipoDocs = await _tipoDocService.GetAllAsync();
            ViewBag.TipoDocumentoId = new SelectList(tipoDocs, "Id_tipo", "Nombre");

            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");
        }
    }
}
