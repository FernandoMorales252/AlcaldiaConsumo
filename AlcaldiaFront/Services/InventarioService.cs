using AlcaldiaFront.DTOs.InventarioDTOs;

namespace AlcaldiaFront.Services
{
    public class InventarioService
    {
        private readonly ApiService _api;
        private const string Base = "Inventario";

        public InventarioService(ApiService api) => _api = api;

        public async Task<List<InventarioRespuestaDTO>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<InventarioRespuestaDTO>(Base, token);
        }

        public async Task<InventarioRespuestaDTO?> GetByIdAsync(int id, string? token = null)
        {
            return await _api.GetByIdAsync<InventarioRespuestaDTO>(Base, id, token);
        }

        public async Task<InventarioRespuestaDTO?> CreateAsync(InventarioCrearDTO dto, string token)
        {
            return await _api.PostAsync<InventarioCrearDTO, InventarioRespuestaDTO>(Base, dto, token);
        }

        public async Task<bool> UpdateAsync(int id, InventarioActualizarDTO dto, string token)
        {
            try
            {
                await _api.PutAsync<InventarioActualizarDTO, InventarioRespuestaDTO>(Base, id, dto, token);
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
