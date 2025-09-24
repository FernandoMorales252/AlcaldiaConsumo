using AlcaldiaFront.DTOs.DocumentoDTOs;

namespace AlcaldiaFront.DTOs.CargoDTOs
{
    public class CargoService
    {

        private readonly ApiService _api;
        private const string Base = "cargo";

        public CargoService(ApiService api) => _api = api;

        // Listar todos los cargos
        public async Task<List<CargoRespuestaDTo>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<CargoRespuestaDTo>(Base, token);
        }

        //Listar un cargo por id
        public async Task<CargoRespuestaDTo?> GetByIdAsync(int id, string? token = null)
        {
            return await _api.GetByIdAsync<CargoRespuestaDTo>(Base, id, token);
        }
        // Crear un nuevo cargo
        public async Task<CargoRespuestaDTo> CreateAsync(CargoCrearDTo dto, string token)
        {
            return await _api.PostAsync<CargoCrearDTo, CargoRespuestaDTo>(Base, dto, token);
        }

        // Actualizar un cargo por id
        public async Task<bool> UpdateAsync(int Id_Cargo, CargoActualizarDTo dto, string token)
        {
            try
            {
                await _api.PutAsync<CargoActualizarDTo, CargoRespuestaDTo>(Base, Id_Cargo, dto, token);
                return true; 
            }
            catch (HttpRequestException)
            {
                return false;  
            }
        }

        // Eliminar un cargo por id
        public async Task<bool> DeleteAsync(int Id_Cargo, string token)
        {
            return await _api.DeleteAsync(Base, Id_Cargo, token);
        }

    }
}

