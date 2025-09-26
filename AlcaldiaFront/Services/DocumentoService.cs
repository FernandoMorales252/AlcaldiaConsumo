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

        public async Task<bool> UpdateAsync(int id, DocumentoActualizarDTO dto, string token)
        {
            // 1. Verificación de seguridad: Asegura que el ID de la ruta (id) y el ID del DTO coincidan.
            if (id != dto.Id_documento)
            {
                throw new ArgumentException($"Error de consistencia. ID de la URL ({id}) no coincide con ID del DTO ({dto.Id_documento}).");
            }

            // 2. Ejecución: Lanza una excepción si falla (404, 400, etc.)
            // Usamos dto.Id_documento para consistencia con el DTO.
            await _api.PutNoContentAsync(Base, dto.Id_documento, dto, token);

            // Si esta línea se alcanza, fue un 204 No Content (Éxito).
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string token)
        {
            return await _api.DeleteAsync(Base, id, token);
        }
    }
}
