using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AlcaldiaFront.DTOs.ProyectoDTOs
{
    public class ProyectoActualizarDTo : IValidatableObject
    {
        [JsonIgnore]
        public int Id_Proyecto { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio.")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "La descripción del proyecto es obligatoria.")]
        public string Descripcion { get; set; } = "";

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        // Valor por defecto para precargar la fecha en la vista de creación
        public DateTime Fecha_Inicio { get; set; } = DateTime.Now.Date;

        [Required(ErrorMessage = "La fecha de finalización es obligatoria.")]
        public DateTime Fecha_Fin { get; set; }

        [Required(ErrorMessage = "El presupuesto es obligatorio.")]
        public decimal Presupuesto { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string Estado { get; set; } = "";

        [Required(ErrorMessage = "El municipio es obligatorio.")]
        public int MunicipioId { get; set; }

        // Implementación de la validación cruzada
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Fecha_Fin.Date < Fecha_Inicio.Date)
            {
                yield return new ValidationResult(
                    "La Fecha de finalización debe ser igual o posterior a la Fecha de inicio.",
                    new[] { nameof(Fecha_Fin) }
                );
            }
        }

    }
}
