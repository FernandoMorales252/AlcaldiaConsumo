using AlcaldiaFront.DTOs.DocumentoDTOs;
using AlcaldiaFront.DTOs.TipoDocumentoDTOs;

namespace AlcaldiaFront.Services
{
    public class TipoDocService
    {
        private readonly ApiService _api;
        private const string Base = "TipoDoc";

        public TipoDocService(ApiService api) => _api = api;

        public async Task<List<TipoDocRespuestaDTO>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<TipoDocRespuestaDTO>(Base, token);
        }


        public async Task<TipoDocRespuestaDTO?> GetByIdAsync(int id, string? token = null)
        {
            return await _api.GetByIdAsync<TipoDocRespuestaDTO>(Base, id, token);
        }


        public async Task<TipoDocRespuestaDTO?> CreateAsync(TipoDocCrearDTO dto, string token)
        {
            return await _api.PostAsync<TipoDocCrearDTO, TipoDocRespuestaDTO>(Base, dto, token);
        }


        public async Task<bool> UpdateAsync(int id, TipoDocActualizarDTO dto, string token)
        {
            try
            {
                await _api.PutAsync<TipoDocActualizarDTO, TipoDocRespuestaDTO>(Base, id, dto, token);
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
