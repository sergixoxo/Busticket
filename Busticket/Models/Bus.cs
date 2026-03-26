using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Busticket.Models
{
    public class Bus
    {
        [Key]
        public int BusId { get; set; }

        // 🔗 RELACIÓN CON EMPRESA
        [ForeignKey("Empresa")]
        public int EmpresaId { get; set; }
        public required Empresa Empresa { get; set; }

        // 🔹 DETALLES DEL BUS
        public string? Placa { get; set; }
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
    }
}