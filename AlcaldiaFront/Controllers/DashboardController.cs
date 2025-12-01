using AlcaldiaFront.DTOs.DashboardDTOs;
using AlcaldiaFront.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlcaldiaFront.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Llama al servicio sin pasar el token.
                var data = await _dashboardService.GetDashboardDataAsync();
                return View(data);
            }
            catch (ApplicationException ex)
            {
                // Capturamos el error de la capa de servicio
                ViewBag.Error = ex.Message;
                // Devolvemos un modelo vacío para que la vista no explote
                return View(new DashboardDataDTO());
            }
            catch (Exception)
            {
                ViewBag.Error = "Ocurrió un error inesperado al cargar el Dashboard.";
                return View(new DashboardDataDTO());
            }
        }
    }

}
