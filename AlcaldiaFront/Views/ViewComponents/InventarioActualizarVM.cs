using Microsoft.AspNetCore.Http;
using AlcaldiaFront.DTOs.InventarioDTOs;
using System.ComponentModel.DataAnnotations;

namespace AlcaldiaFront.ViewModels
{
    public class InventarioActualizarVM : InventarioActualizarDTO
    {
        // Campo IFormFile para recibir una nueva imagen
        [Display(Name = "Nueva Imagen (opcional)")]
        public IFormFile? ArchivoImagen { get; set; }

        // Propiedad adicional para guardar la imagen Base64 existente (solo para mostrarla en la vista de edición)
        public string? ImagenDataExistente { get; set; }
    }
}