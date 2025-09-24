using AlcaldiaFront.DTOs.ProyectoDTOs;

namespace AlcaldiaFront.Services
{
    public class ProyectoService
    {
        private readonly ApiService _api;
        private const string Base = "Proyecto";

        public ProyectoService(ApiService api) => _api = api;

        public async Task<List<ProyectoRespuestaDTo>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<ProyectoRespuestaDTo>(Base, token);
        }

        public async Task<ProyectoRespuestaDTo?> GetByIdAsync(int id, string? token = null)
        {
            return await _api.GetByIdAsync<ProyectoRespuestaDTo>(Base, id, token);
        }

        public async Task<ProyectoRespuestaDTo> CreateAsync(ProyectoCrearDTo dto, string token)
        {
            return await _api.PostAsync<ProyectoCrearDTo, ProyectoRespuestaDTo>(Base, dto, token);
        }

        public async Task<bool> UpdateAsync(int id, ProyectoActualizarDTo dto, string token)
        {
            try
            {
                await _api.PutAsync<ProyectoActualizarDTo, ProyectoRespuestaDTo>(Base, id, dto, token);
                return true; // Si la llamada es exitosa, retorna true.
            }
            catch (HttpRequestException)
            {
                return false;  // Captura la excepción si la solicitud no fue exitosa (por ejemplo, 404 Not Found o 400 Bad Request).
            }
        }

        public async Task<bool> DeleteAsync(int id, string token)
        {
            return await _api.DeleteAsync(Base, id, token);
        }
    }
}
