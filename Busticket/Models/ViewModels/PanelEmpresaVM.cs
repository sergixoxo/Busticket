using Busticket.Models;
using System.Collections.Generic;

namespace Busticket.Models.ViewModels
{
    public class PanelEmpresaVM
    {
        // ✅ Propiedad obligatoria
        public required Empresa Empresa { get; set; }

        // ✅ Lista inicializada para evitar null
        public required List<Venta> Ventas { get; set; } = new();
    }
}