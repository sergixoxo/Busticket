using System;

namespace Busticket.Models.ViewModels
{
    public class ReporteVentaVM1
    {
        public int VentaId { get; set; }
        public required string Usuario { get; set; }
        public required string Empresa { get; set; }
        public decimal Precio { get; set; }
        public DateTime Fecha { get; set; }
    }
}