
using AlcaldiaFront.DTOs.InventarioDTOs;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AlcaldiaFront.ViewModels // O la carpeta que uses para ViewModels
{
    // Heredamos del DTO de la API para que el mapeo de campos sea automático
    public class InventarioCrearVM : InventarioCrearDTO
    {
        // Campo IFormFile para recibir el archivo subido desde el formulario.
        // Este campo NO se mapea a la API; solo se usa en el Controller.
        [Display(Name = "Imagen")]
        public IFormFile? ArchivoImagen { get; set; }
    }
}
