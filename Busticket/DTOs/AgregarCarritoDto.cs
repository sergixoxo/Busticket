using Busticket.Models;
using System.Collections.Generic;

namespace Busticket.DTOs
{
    public class CarritoItem
    {
        public int AsientoId { get; set; }
        public string Codigo { get; set; }
        public decimal Precio { get; set; }
        public int RutaId { get; set; }
    }

    public class EmailService
    {
        public int RutaId { get; set; }
        public List<CarritoItem> Asientos { get; set; }
    }
}
