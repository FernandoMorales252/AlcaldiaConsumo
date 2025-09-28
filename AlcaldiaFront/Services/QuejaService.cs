using AlcaldiaFront.DTOs.QuejaDTOs;

namespace AlcaldiaFront.Services
{
    public class QuejaService
    {
        private readonly ApiService _api;
        private const string Base = "Queja";
        public QuejaService(ApiService api) => _api = api;

        public async Task<List<QuejaRespuestaDTO>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<QuejaRespuestaDTO>(Base, token);
        }

        public async Task<QuejaRespuestaDTO?> GetByIdAsync(int Id_queja, string? token = null)
        {
            return await _api.GetByIdAsync<QuejaRespuestaDTO>(Base, Id_queja, token);
        }

        public async Task<QuejaRespuestaDTO?> CreateAsync(QuejaCrearDTO dto, string token)
        {
            return await _api.PostAsync<QuejaCrearDTO, QuejaRespuestaDTO>(Base, dto, token);
        }

        public async Task<bool> UpdateAsync(int Id_queja, QuejaActualizarDTO dto, string token)
        {
            try
            {
                await _api.PutAsync<QuejaActualizarDTO, QuejaRespuestaDTO>(Base, Id_queja, dto, token);
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int Id_queja, string token)
        {
            return await _api.DeleteAsync(Base, Id_queja, token);
        }
    }
}
