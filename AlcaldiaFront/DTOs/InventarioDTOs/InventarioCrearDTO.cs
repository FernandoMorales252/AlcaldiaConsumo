namespace AlcaldiaFront.DTOs.InventarioDTOs
{
    public class InventarioCrearDTO
    {
        public string Nombre_item { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Cantidad { get; set; }
        public DateTime Fecha_ingreso { get; set; } = DateTime.Now;
        public string Estado { get; set; }
        public int MunicipioId { get; set; }
    }
}
