using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Busticket.Models
{
    public class Resena
    {
        [Key]
        public int ResenaId { get; set; }

        // Relación con AspNetUsers (Identity)
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        // Relación con Ruta
        public int RutaId { get; set; }
        public Ruta Ruta { get; set; }

        public int Calificacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }
}