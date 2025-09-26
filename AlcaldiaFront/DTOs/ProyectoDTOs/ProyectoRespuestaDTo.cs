namespace AlcaldiaFront.DTOs.ProyectoDTOs
{
    public class ProyectoRespuestaDTo
    {
        public int Id_Proyecto { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public decimal Presupuesto { get; set; }
        public string Estado { get; set; } = "";
        public int MunicipioId { get; set; }
    }
}
