namespace AlcaldiaFront.DTOs.DocumentoDTOs
{
    public class DocumentoActualizarDTO
    {
        public int Id_documento { get; set; }
        public string Numero_documento { get; set; } = "";
        public DateTime Fecha_emision { get; set; }
        public string Propietario { get; set; } = "";
        public string Detalles { get; set; } = "";
        public string Estado { get; set; }
        public int TipoDocumentoId { get; set; }
        public int MunicipioId { get; set; }
    }
}
