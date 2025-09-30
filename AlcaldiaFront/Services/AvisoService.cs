using AlcaldiaFront.DTOs.AvisoDTOs;

namespace AlcaldiaFront.Services
{
    public class AvisoService
    {
        private readonly ApiService _api;
        private const string Base = "Aviso";
        public AvisoService(ApiService api) => _api = api;

        public async Task<List<AvisoRespuestaDTO>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<AvisoRespuestaDTO>(Base, token);
        }

        public async Task<AvisoRespuestaDTO?> GetByIdAsync(int Id_aviso, string? token = null)
        {
            return await _api.GetByIdAsync<AvisoRespuestaDTO>(Base, Id_aviso, token);
        }

        public async Task<AvisoRespuestaDTO?> CreateAsync(AvisoCrearDTO dto, string token)
        {
            return await _api.PostAsync<AvisoCrearDTO, AvisoRespuestaDTO>(Base, dto, token);
        }

        public async Task<bool> UpdateAsync(int Id_aviso, AvisoActualizarDTO dto, string token)
        {
            try
            {
                await _api.PutAsync<AvisoActualizarDTO, AvisoRespuestaDTO>(Base, Id_aviso, dto, token);
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }



        public async Task<bool> DeleteAsync(int Id_aviso, string token)
        {
            return await _api.DeleteAsync(Base, Id_aviso, token);
        }
    }
}
