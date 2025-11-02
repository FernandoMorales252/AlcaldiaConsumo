namespace AlcaldiaFront.DTOs.AvisoDTOs
{
    public class AvisoCrearDTO
    {
        public string Titulo { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public DateTime Fecha_Registro { get; set; } = DateTime.Now;

        public string Tipo { get; set; } = "";

        public int MunicipioId { get; set; }
    }
}
