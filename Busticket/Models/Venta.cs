using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Busticket.Models
{
    public class Venta
    {
        public int VentaId { get; set; }

        // ⚠️ Propiedades obligatorias
        public required string UserId { get; set; }
        public required IdentityUser User { get; set; }

        public int EmpresaId { get; set; }
        public required Empresa Empresa { get; set; }

        public int RutaId { get; set; }
        public required Ruta Ruta { get; set; }

        // 💰 Total pagado por TODA la compra
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public DateTime Fecha { get; set; }

        // 🧾 Una venta tiene muchos boletos
        public required ICollection<Boleto> Boletos { get; set; }
    }
}