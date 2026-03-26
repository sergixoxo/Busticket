using Busticket.Models;
using System.Collections.Generic;

namespace Busticket.DTOs
{
    public class CarritoItem
    {
        public int AsientoId { get; set; }

        // ⚠️ Propiedad obligatoria
        public required string Codigo { get; set; }

        public decimal Precio { get; set; }
        public int RutaId { get; set; }
    }

    public class EmailServiceDTO
    {
        public int RutaId { get; set; }

        // ⚠️ Lista obligatoria, inicializada
        public required List<CarritoItem> Asientos { get; set; } = new();
    }
}