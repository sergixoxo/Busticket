using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Busticket.Models
{
    public class Asiento
    {
        [Key]
        public int AsientoId { get; set; }

        [Required]
        public int Numero { get; set; }

        // ✅ AÑADIDO: Propiedad necesaria para identificar el asiento (ej: "A1", "B2")
        [Required]
        public string Codigo { get; set; } = string.Empty;

        // 🔥 ESTADOS
        public bool Disponible { get; set; } = true;
        public bool Reservado { get; set; } = false;

        // 🔐 CONTROL DE RESERVA
        public string? ReservadoPorUserId { get; set; }
        public DateTime? FechaReserva { get; set; }

        // 🔗 RELACIÓN CON RUTA
        [Required]
        public int RutaId { get; set; }

        [ForeignKey("RutaId")]
        [ValidateNever]
        public virtual Ruta? Ruta { get; set; }
    }
}