using System.ComponentModel.DataAnnotations;

namespace AlcaldiaFront.WebApp.DTOs.UsuarioDTOs
{
    public class UsuarioLoginDTO
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

