using AlcaldiaFront.DTOs.EmpleadoDTOs;

namespace AlcaldiaFront.Services
{
    public class EmpleadoService
    {
        private readonly ApiService _api;
        private const string Base = "empleado";
        public EmpleadoService(ApiService api) => _api = api;

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

    }
}
