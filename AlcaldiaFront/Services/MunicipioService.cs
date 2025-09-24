using AlcaldiaFront.DTOs.MunicipioDTOs;

namespace AlcaldiaFront.Services
{
    public class MunicipioService
    {
        private readonly ApiService _api;
        private const string Base = "Municipio";

        public MunicipioService(ApiService api) => _api = api;

        public async Task<List<MunicipioRespuestaDTO>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<MunicipioRespuestaDTO>(Base, token);
        }

        public async Task<MunicipioRespuestaDTO?> GetByIdAsync(int id, string? token = null)
        {
            return await _api.GetByIdAsync<MunicipioRespuestaDTO>(Base, id, token);
        }

        public async Task<MunicipioRespuestaDTO?> CreateAsync(MunicipioCrearDTO dto, string token)
        {
            return await _api.PostAsync<MunicipioCrearDTO, MunicipioRespuestaDTO>(Base, dto, token);
        }

        public async Task<bool> UpdateAsync(int id, MunicipioActualizarDTO dto, string token)
        {
            try
            {
                await _api.PutAsync<MunicipioActualizarDTO, MunicipioRespuestaDTO>(Base, id, dto, token);
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
