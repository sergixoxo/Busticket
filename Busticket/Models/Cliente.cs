using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Busticket.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Documento { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        // 🔗 Relación con Identity
        public IdentityUser User { get; set; } = null!;
    }
}