using Busticket.Models;
using System.Collections.Generic;

namespace Busticket.DTOs
{
    public class EmailServiceDTO
    {
        public int RutaId { get; set; }

        // Lista de asientos a enviar
        public required List<CarritoItem> Asientos { get; set; } = new();
    }


}