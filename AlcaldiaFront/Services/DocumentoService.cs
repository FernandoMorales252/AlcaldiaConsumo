using AlcaldiaFront.DTOs.DocumentoDTOs;

namespace AlcaldiaFront.Services
{
    public class DocumentoService
    {
        private readonly ApiService _api;
        private const string Base = "documentoarchivo";

        public DocumentoService(ApiService api) => _api = api;

        public async Task<List<DocumentoRespuestaDTo>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<DocumentoRespuestaDTo>(Base, token);
        }


        public async Task<DocumentoRespuestaDTo?> GetByIdAsync(int id, string? token = null)
        {
            return await _api.GetByIdAsync<DocumentoRespuestaDTo>(Base, id, token);
        }


        public async Task<DocumentoRespuestaDTo?> CreateAsync(DocumentoCrearDTo dto, string token)
        {
            return await _api.PostAsync<DocumentoCrearDTo, DocumentoRespuestaDTo>(Base, dto, token);
        }


        public async Task<bool> UpdateAsync(int id, DocumentoActualizarDTo dto, string token)
        {
            try
            {
                await _api.PutAsync<DocumentoActualizarDTo, DocumentoRespuestaDTo>(Base, id, dto, token);
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
