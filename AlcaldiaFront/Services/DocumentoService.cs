using AlcaldiaFront.DTOs.CargoDTOs;
using AlcaldiaFront.DTOs.DocumentoDTOs;
using AlcaldiaFront.DTOs.MunicipioDTOs;

namespace AlcaldiaFront.Services
{
    public class DocumentoService
    {
        private readonly ApiService _api;
        private const string Base = "Documento";

        public DocumentoService(ApiService api) => _api = api;

        public async Task<List<DocumentoRespuestaDTO>?> GetAllAsync(string? token = null)
        {
            return await _api.GetAllAsync<DocumentoRespuestaDTO>(Base, token);
        }

        public async Task<DocumentoRespuestaDTO?> GetByIdAsync(int id, string? token = null)
        {
            return await _api.GetByIdAsync<DocumentoRespuestaDTO>(Base, id, token);
        }

        public async Task<DocumentoRespuestaDTO?> CreateAsync(DocumentoCrearDTO dto, string token)
        {
            return await _api.PostAsync<DocumentoCrearDTO, DocumentoRespuestaDTO>(Base, dto, token);
        }

        public async Task<bool> UpdateAsync(int Id_documento, DocumentoActualizarDTO dto, string token)
        {
            try
            {
                await _api.PutAsync<DocumentoActualizarDTO, DocumentoRespuestaDTO>(Base, Id_documento, dto, token);
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, string token)
        {
            return await _api.DeleteAsync(Base, id, token);
        }
    }
}
