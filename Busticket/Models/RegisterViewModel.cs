using System.ComponentModel.DataAnnotations;

namespace Busticket.Models
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
        public string TipoUsuario { get; set; } = "Usuario";

        // ================= EMAIL =================
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        public string Email { get; set; } = string.Empty;

        // ================= PASSWORD =================
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener mínimo 8 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$",
            ErrorMessage = "Debe contener mayúscula, minúscula y número")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // ================= CLIENTE =================
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Ingresa nombre y apellido completo (mínimo 8 caracteres)")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$",
            ErrorMessage = "El nombre solo puede contener letras")]
        public string? Nombre { get; set; }

        // ================= EMPRESA =================
        [StringLength(150, ErrorMessage = "El nombre de la empresa es muy largo")]
        public string? NombreEmpresa { get; set; }

        [RegularExpression(@"^\d{9}-\d{1}$",
            ErrorMessage = "El NIT debe tener formato 900123456-9")]
        public string? Nit { get; set; }

        // ================= TELÉFONO =================
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "El teléfono debe contener 10 dígitos numéricos")]
        public string? Telefono { get; set; }

        // ================= VALIDACIÓN CONDICIONAL =================
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipoUsuario == "Usuario")
            {
                if (string.IsNullOrWhiteSpace(Nombre))
                {
                    yield return new ValidationResult(
                        "El nombre es obligatorio para clientes",
                        new[] { nameof(Nombre) });
                }
            }

            if (TipoUsuario == "Empresa")
            {
                if (string.IsNullOrWhiteSpace(NombreEmpresa))
                {
                    yield return new ValidationResult(
                        "El nombre de la empresa es obligatorio",
                        new[] { nameof(NombreEmpresa) });
                }

                if (string.IsNullOrWhiteSpace(Nit))
                {
                    yield return new ValidationResult(
                        "El NIT es obligatorio",
                        new[] { nameof(Nit) });
                }
            }
        }
    }
}