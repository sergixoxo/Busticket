using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Busticket.Models
{
    public class Boleto
    {
        [Key]
        public int BoletoId { get; set; }

        public int VentaId { get; set; }
        public required Venta Venta { get; set; }

        public required string UserId { get; set; }
        public required IdentityUser User { get; set; }

        public int RutaId { get; set; }
        public required Ruta Ruta { get; set; }

        public int AsientoId { get; set; }
        public required Asiento Asiento { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public DateTime FechaCompra { get; set; }

        public required string Codigo { get; set; }
    }
}