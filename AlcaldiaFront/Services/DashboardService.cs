using AlcaldiaFront.DTOs.DashboardDTOs;

namespace AlcaldiaFront.Services
{
    public class DashboardService
    {
        private readonly ApiService _apiService;
        // La ruta relativa que apunta a tu controlador API backend
        private const string DashboardEndpoint = "Dashboard/data";

        public DashboardService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // AlcaldiaFront/Services/DashboardService.cs (Bloque Catch Corregido)

        public async Task<DashboardDataDTO> GetDashboardDataAsync()
        {
            try
            {
                var data = await _apiService.GetSimpleAsync<DashboardDataDTO>(DashboardEndpoint, null);
                return data ?? new DashboardDataDTO();
            }
            // 1. Capturar la excepción específica de solicitud HTTP (4xx, 5xx)
            catch (HttpRequestException httpEx)
            {
                // En este punto, el HandleResponse en ApiService ya ha llamado a response.EnsureSuccessStatusCode(),
                // y esta excepción httpEx contiene la respuesta de error de la API (401, 500, etc.).
                string statusCode = httpEx.StatusCode.HasValue ? $"HTTP {httpEx.StatusCode.Value}" : "Error de Red/Desconocido";

                // Loguea el error en consola para depuración
                Console.WriteLine($"[DashboardService ERROR] {statusCode}: {httpEx.Message}");

                // Lanzar una ApplicationException con el detalle del error HTTP
                throw new ApplicationException($"Fallo de comunicación con la API: {statusCode}. (Verifique URL Base/Token/CORS).", httpEx);
            }
            // 2. Capturar cualquier otra excepción inesperada (ej. error de deserialización, etc.)
            catch (Exception ex)
            {
                string errorMessage = "Ocurrió un error inesperado al procesar los datos del Dashboard.";

                // Si hay un InnerException (como un error de deserialización), inclúyelo.
                if (ex.InnerException != null)
                {
                    errorMessage += $" Detalle interno: {ex.InnerException.Message}";
                }

                // Loguea el error en consola para depuración
                Console.WriteLine($"[DashboardService ERROR FATAL] {ex.Message}");

                // Lanzar la ApplicationException con el mensaje más útil
                throw new ApplicationException(errorMessage, ex);
            }
        }
    }
}
