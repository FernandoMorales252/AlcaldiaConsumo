using System.Text.Json.Serialization;

namespace AlcaldiaFront.DTOs.MunicipioDTOs
{
    public class MunicipioActualizarDTO
    {
        [JsonIgnore]
        public int Id_Municipio { get; set; }
        public string Nombre_Municipio { get; set; } = "";
    }
}
