using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Busticket.Models
{
    [Table("Ruta")]
    public class Ruta
    {
        [Key]
        public int RutaId { get; set; }

        // Ciudad Origen
        [Required]
        public int CiudadOrigenId { get; set; }

        [ForeignKey("CiudadOrigenId")]
        [ValidateNever]
        public Ciudad CiudadOrigen { get; set; }

        // Ciudad Destino
        [Required]
        public int CiudadDestinoId { get; set; }
        [ValidateNever]
        [ForeignKey("CiudadDestinoId")]
        public Ciudad CiudadDestino { get; set; }

        // Empresa

        public int EmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        [ValidateNever]
        public Empresa Empresa { get; set; }

        // Precio
        [Required]
        public decimal Precio { get; set; }

        // Duración
        [Required]
        public int DuracionMin { get; set; }

        // 🆕 FECHAS
   

[DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime FechaSalida { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime FechaLlegada { get; set; }

    public string? ImagenUrl { get; set; }

        public List<Asiento>? Asiento { get; set; }
    }
}