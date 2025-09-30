using System.Text.Json.Serialization;

namespace AlcaldiaFront.DTOs.CargoDTOs
{
    public class CargoActualizarDTo
    {
        [JsonIgnore]
        public int Id_Cargo { get; set; }
        public string Nombre_cargo { get; set; } = "";
        public string Descripcion { get; set; } = "";
    }
}
