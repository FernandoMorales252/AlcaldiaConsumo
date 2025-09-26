namespace AlcaldiaFront.DTOs.EmpleadoDTOs
{
    public class EmpleadoActualizarDTo
    {
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public DateTime Fecha_contratacion { get; set; }
        public string Estado { get; set; }
        public int CargoId { get; set; }
        public int MunicipioId { get; set; }
    }
}

