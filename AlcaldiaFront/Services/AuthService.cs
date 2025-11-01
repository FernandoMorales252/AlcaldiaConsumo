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
        return await _apiService.PostAsync<UsuarioLoginDTO, LoginResponseDTO>("auth/login", dto);
    }
    public async Task<LoginResponseDTO> RegistrarAsync(UsuarioRegistroDTO dto)
    {
        return await _apiService.PostAsync<UsuarioRegistroDTO, LoginResponseDTO>("auth/registrar", dto);
    }
}