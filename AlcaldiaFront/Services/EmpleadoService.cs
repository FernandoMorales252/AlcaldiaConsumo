using AlcaldiaFront.DTOs.EmpleadoDTOs;
using System.Net.Http; // Necesario para HttpClient

namespace AlcaldiaFront.Services
{
    public class EmpleadoService
    {
        private readonly ApiService _api;
        private const string Base = "empleado";
        // Mantener _httpClient por si se usa en otros lugares, pero NO lo usaremos para exportar.
        private readonly HttpClient _httpClient;

        public EmpleadoService(ApiService api, HttpClient httpClient)
        {
            _api = api;
            _httpClient = httpClient;
        }

        public async Task<List<EmpleadoRespuestaDTo>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<EmpleadoRespuestaDTo>(Base, token);
        }

        public async Task<EmpleadoRespuestaDTo?> GetByIdAsync(int Id_empleado, string? token = null)
        {
            return await _api.GetByIdAsync<EmpleadoRespuestaDTo>(Base, Id_empleado, token);
        }

        public async Task<EmpleadoRespuestaDTo?> CreateAsync(EmpleadoCrearDTo dto, string token)
        {
            return await _api.PostAsync<EmpleadoCrearDTo, EmpleadoRespuestaDTo>(Base, dto, token);
        }

        public async Task<bool> UpdateAsync(int Id_empleado, EmpleadoActualizarDTo dto, string token)
        {
            try
            {
                await _api.PutAsync<EmpleadoActualizarDTo, EmpleadoRespuestaDTo>(Base, Id_empleado, dto, token);
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int Id_empleado, string token)
        {
            return await _api.DeleteAsync(Base, Id_empleado, token);
        }


        // === MÉTODO CRÍTICO SIMPLIFICADO ===
        public async Task<byte[]?> ExportarExcelAsync(string token)
        {
            Console.WriteLine("--- EJECUTANDO EXPORTAR EXCEL ASYNC (VIA API SERVICE) ---");

            // 1. Definir la ruta relativa. ApiService usará su BaseAddress configurado.
            var endpoint = $"{Base}/ExportarExcel";

            try
            {
                // 2. Usar el nuevo método en ApiService, que usa el HttpClient CONFIGURADO.
                // Esto resuelve el error de URI.
                var fileBytes = await _api.GetFileBytesAsync(endpoint, token);

                if (fileBytes == null)
                {
                    Console.WriteLine("ADVERTENCIA: Fallo en la descarga del archivo. Revisar logs de ApiService.");
                }

                return fileBytes;
            }
            catch (Exception ex)
            {
                // Capturamos cualquier otra excepción
                Console.WriteLine($"Excepción en EmpleadoService.ExportarExcelAsync: {ex.Message}");
                return null;
            }
        }
    }
}




