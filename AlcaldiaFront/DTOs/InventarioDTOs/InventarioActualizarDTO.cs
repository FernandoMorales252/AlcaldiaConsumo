namespace AlcaldiaFront.DTOs.InventarioDTOs
{
    public class InventarioActualizarDTO
    {
        public int Id_inventario { get; set; }
        public string Nombre_item { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Cantidad { get; set; }
        public DateTime Fecha_ingreso { get; set; }
        public string Estado { get; set; }
        public int MunicipioId { get; set; }
    }
}
