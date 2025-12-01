using System.Collections.Generic;

namespace AlcaldiaFront.DTOs.DashboardDTOs
{
    public class DashboardDataDTO
    {
        public Dictionary<string, int> EmpleadosPorCargo { get; set; }
        public Dictionary<string, int> EmpleadosPorMunicipio { get; set; }
        public int TotalEmpleadosActivos { get; set; }

        // --- 2. Proyectos ---
        public Dictionary<string, int> ProyectosPorEstado { get; set; }
        public Dictionary<string, decimal> PresupuestoPorMunicipio { get; set; }

        // --- 3. Inventario ---
        public Dictionary<string, int> InventarioPorEstado { get; set; } // Cantidad de items por estado
        public List<ItemCriticoDTO> TopItemsAgotados { get; set; }
        public int TotalItemsEnBaja { get; set; }

        // --- 4. Documentos y Quejas ---
        public Dictionary<string, int> QuejasPorTipo { get; set; }
        public int TotalDocumentosVigentes { get; set; }
    }

    public class ItemCriticoDTO
    {
        public string NombreItem { get; set; }
        public int Cantidad { get; set; }
    }
}

