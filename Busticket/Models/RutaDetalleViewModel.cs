using Busticket.Models;
using System.Collections.Generic;

namespace Busticket.Models
{
    public class ReporteVentaVM
    {
        // ✅ Ruta siempre requerida
        public required Ruta Ruta { get; set; }

        // ✅ Lista inicializada para evitar null
        public List<Asiento> Asientos { get; set; } = new List<Asiento>();

        // Opcional, puede ser null
        public string? RequestId { get; set; }

        // Propiedad calculada
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}