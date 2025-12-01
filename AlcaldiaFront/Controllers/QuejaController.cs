using AlcaldiaFront.DTOs.DocumentoDTOs;
using AlcaldiaFront.DTOs.ProyectoDTOs;
using AlcaldiaFront.DTOs.QuejaDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlcaldiaFront.Controllers
{
    [Authorize]
    public class QuejaController : Controller
    {
        private readonly QuejaService _quejaService;
        private readonly MunicipioService _municipioService;

        public QuejaController(QuejaService quejaService, MunicipioService municipioService)
        {
            _quejaService = quejaService;
            _municipioService = municipioService;
        }


        public async Task<IActionResult> Index(int? MunicipioId, string Tipo, string Nivel, string Reporte)
        {
            try
            {
                var reportes = await _quejaService.GetAllAsync();

                await PopulateFilterDropdowns();

                if (reportes != null && reportes.Any())
                {
                    var AFiltrados = reportes.AsQueryable();

                    if (MunicipioId.HasValue && MunicipioId.Value > 0)
                    {
                        AFiltrados = AFiltrados.Where(d => d.MunicipioId == MunicipioId.Value);
                    }

                    if (!string.IsNullOrEmpty(Tipo) && Tipo != "Todos")
                    {
                        AFiltrados = AFiltrados.Where(d => d.Tipo.Equals(Tipo, StringComparison.OrdinalIgnoreCase));
                    }

                    if (!string.IsNullOrEmpty(Nivel) && Nivel != "Todos")
                    {
                        AFiltrados = AFiltrados.Where(d => d.Nivel.Equals(Nivel, StringComparison.OrdinalIgnoreCase));
                    }

                    if (!string.IsNullOrEmpty(Reporte))
                    {
                        AFiltrados = AFiltrados.Where(d =>
                            d.Titulo != null &&
                            d.Titulo.Contains(Reporte, StringComparison.OrdinalIgnoreCase));
                    }

                    ViewBag.CurrentMunicipioId = MunicipioId;
                    ViewBag.CurrentEstado = Tipo;
                    ViewBag.CurrentNivel = Nivel;
                    ViewBag.CurrentTitulo = Reporte;

                    return View(AFiltrados.ToList());
                }

                // Devolvemos una lista de QuejaRespuestaDTO si no hay resultados o si la lista es nula
                return View(new List<QuejaRespuestaDTO>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar los reportes: " + ex.Message;
                await PopulateFilterDropdowns(); // Asegurar que los ViewBag estén llenos incluso con error
                return View(new List<QuejaRespuestaDTO>());
            }
        }

        public async Task<IActionResult> Menu()
        {
            try
            {
                var avisos = await _quejaService.GetAllAsync("your_access_token");

                // Obtener listas para los nombres de los municipios y cargos
                var municipios = await _municipioService.GetAllAsync();

                // Convertir las listas a diccionarios para la vista
                ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);

                return View(avisos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudieron cargar las quejas: " + ex.Message;
                // Si hay un error, inicializar los ViewBag para evitar NullReferenceException
                ViewBag.MunicipioNombres = new Dictionary<int, string>();
                return View(new List<QuejaRespuestaDTO>());
            }
        }


        // GET: Empleado/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var queja = await _quejaService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            if (queja == null)
            {
                return NotFound();
            }
            return View(queja);
        }

        // GET: Empleado/Create


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var queja = await _quejaService.GetByIdAsync(id);
            if (queja == null)
            {
                return NotFound();
            }

            var dto = new QuejaActualizarDTO
            {
                Id_queja = queja.Id_queja,
                Titulo = queja.Titulo,
                Descripcion = queja.Descripcion,
                Fecha_Registro = queja.Fecha_Registro,
                Tipo = queja.Tipo,
                Nivel = queja.Nivel,
                MunicipioId = queja.MunicipioId
            };
            await PopulateDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuejaActualizarDTO queja)
        {

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(queja);
            }
            var success = await _quejaService.UpdateAsync(id, queja, "");
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al actualizar el Reporte.");
            await PopulateDropdowns();
            return View(queja);
        }

        // GET: Empleado/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var queja = await _quejaService.GetByIdAsync(id);
            // Obtener listas para los nombres de los municipios y cargos
            var municipios = await _municipioService.GetAllAsync();

            // Convertir las listas a diccionarios para la vista
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
            if (queja == null)
            {
                return NotFound();
            }
            return View(queja);
        }

        // POST: Empleado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _quejaService.DeleteAsync(id, "your_access_token");
                if (success)
                {
                    TempData["Ok"] = "Reporte eliminado con éxito.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error al eliminar el aviso.");
                return View("Delete", await _quejaService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el aviso: " + ex.Message);
                return View("Delete", await _quejaService.GetByIdAsync(id));
            }
        }

        private async Task PopulateFilterDropdowns()
        {
            // Cargar listas para filtros
            var municipios = await _municipioService.GetAllAsync();

            // 1. Dropdowns para la sección de Filtro (con opción "Todos")
            // Municipio
            var municipiosList = municipios.Select(m => new SelectListItem
            {
                Value = m.Id_Municipio.ToString(),
                Text = m.Nombre_Municipio
            }).ToList();
            municipiosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Municipios" });
            ViewBag.MunicipioId = municipiosList;


            // Estado (Tipo) - Estatica
            var tiposList = new List<SelectListItem>
            {
                 new SelectListItem { Value = "", Text = "Todos los Tipos" }, // Etiqueta corregida
                 new SelectListItem { Value = "desechos", Text = "Desechos" },
                 new SelectListItem { Value = "contaminacion", Text = "Contaminacion" },
                 new SelectListItem { Value = "seguridad", Text = "Seguridad" },
                 new SelectListItem { Value = "infraestructura", Text = "Infraestructura" },
                 new SelectListItem { Value = "otro", Text = "Otro" }
            };

            // Nivel - Estatica
            var nivelesList = new List<SelectListItem> {
                 new SelectListItem { Value = "", Text = "Todos los Niveles" },
                 new SelectListItem { Value = "bajo", Text = "Bajo" },
                 new SelectListItem { Value = "medio", Text = "Medio" },
                 new SelectListItem { Value = "alto", Text = "Alto" },
                 new SelectListItem { Value = "riesgo", Text = "Riesgo" }
             };

            ViewBag.TipoList = tiposList;
            ViewBag.NivelList = nivelesList; 

            // 2. Diccionarios de Nombres (para mostrar en la tabla de resultados)
            ViewBag.MunicipioNombres = municipios.ToDictionary(m => m.Id_Municipio, m => m.Nombre_Municipio);
        }

        private async Task PopulateDropdowns()
        {
            // Carga y configura los dropdowns para Municipio, Tipo y Nivel (para Create/Edit)
            var municipios = await _municipioService.GetAllAsync();
            ViewBag.MunicipioId = new SelectList(municipios, "Id_Municipio", "Nombre_Municipio");

            // Listas estáticas para Tipos y Niveles
            ViewBag.Tipo = new SelectList(new List<string> { "desechos", "contaminacion", "seguridad", "infraestructura", "otro" });
            ViewBag.Nivel = new SelectList(new List<string> { "bajo", "medio", "alto", "riesgo" });
        }
    }
}
