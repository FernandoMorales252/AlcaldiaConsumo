using AlcaldiaFront.Services;
using AlcaldiaFront.WebApp.DTOs.UsuarioDTOs;

namespace AlcaldiaFront.WebApp.Services;


public class AuthService
{
    private readonly ApiService _apiService;
    public AuthService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<LoginResponseDTO?> LoginAsync(UsuarioLoginDTO dto)
    {
        try
        {
            // Intentar llamar al API
            return await _apiService.PostAsync<UsuarioLoginDTO, LoginResponseDTO>("auth/login", dto);
        }
        catch (HttpRequestException ex)
        {
            // El API service lanza una excepción por el 401 (Unauthorized).
            // Si el mensaje contiene "401", ignoramos la excepción y retornamos null
            // para que el controlador pueda manejar el error de credenciales.
            if (ex.Message.Contains("401 (Unauthorized)"))
            {
                return null;
            }

            // Si es cualquier otro error (500, red, etc.), lanzamos la excepción
            // para que se maneje como un error más grave.
            throw;
        }
    }

    public async Task<LoginResponseDTO> RegistrarAsync(UsuarioRegistroDTO dto)
    {
        // El bloque try-catch ya fue añadido en el controlador para este método,
        // asumiendo que el error de duplicidad (409 Conflict o similar) también
        // lanza un HttpRequestException.
        return await _apiService.PostAsync<UsuarioRegistroDTO, LoginResponseDTO>("auth/registrar", dto);
    }
}