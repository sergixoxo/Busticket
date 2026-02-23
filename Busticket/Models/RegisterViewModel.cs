using System.ComponentModel.DataAnnotations;

namespace Busticket.Models
{
    public class RegisterViewModel
    {
        /* 🔹 CONTROL */
        [Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
        public string TipoUsuario { get; set; } = "Cliente";

        /* 🔹 CAMPOS COMUNES */
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /* 🔹 PERSONA (Cliente / Conductor) */
        public string? Nombre { get; set; }

        /* 🔹 EMPRESA (SOLO SI ES EMPRESA) */
        public string? NombreEmpresa { get; set; }
        public string? Nit { get; set; }
        public string? Telefono { get; set; }
    }
}
