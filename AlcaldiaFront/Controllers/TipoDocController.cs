using AlcaldiaFront.DTOs.TipoDocumentoDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlcaldiaFront.Controllers
{
    public class TipoDocController : Controller
    {
        private readonly TipoDocService _svc;

        public TipoDocController(TipoDocService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _svc.GetAllAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<TipoDocRespuestaDTO>());
            }
        }

    }
}
