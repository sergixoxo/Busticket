using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Busticket.Models
{
    public class Asiento
    {
        [Key]
        public int AsientoId { get; set; }

        [Required]
        public int Numero { get; set; }

        // 🔥 ESTADOS
        public bool Disponible { get; set; } = true;
        public bool Reservado { get; set; } = false;

        // 🔐 CONTROL DE RESERVA
        public string? ReservadoPorUserId { get; set; }
        public DateTime? FechaReserva { get; set; }

        [Required]
        public int RutaId { get; set; }

        [ForeignKey("RutaId")]
        public Ruta Ruta { get; set; } = null!;
    }
}