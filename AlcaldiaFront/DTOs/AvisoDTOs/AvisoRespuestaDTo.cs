namespace AlcaldiaFront.DTOs.AvisoDTOs
{
    public class AvisoRespuestaDTO
    {
        public int Id_aviso { get; set; }

        public string Titulo { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public DateTime Fecha_Registro { get; set; }

        public string Tipo { get; set; } = "";

        public int MunicipioId { get; set; }
    }
}
