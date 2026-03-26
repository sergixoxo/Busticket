using Busticket.Models;
using System.Collections.Generic;

namespace Busticket.DTOs
{
    public class CarritoItem
    {
        public int AsientoId { get; set; }

        // ⚠️ Propiedad non-nullable
        public required string Codigo { get; set; }

        public decimal Precio { get; set; }
        public int RutaId { get; set; }
    }

    public class EmailService
    {
        public int RutaId { get; set; }

        // ⚠️ Lista inicializada para evitar null
        public required List<CarritoItem> Asientos { get; set; } = new();
    }
}