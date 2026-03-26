using Busticket.Models;

namespace Busticket.Models.ViewModels
{
    public class PanelEmpresaVM
    {
        // ⚠️ Propiedades obligatorias
        public required Empresa Empresa { get; set; }
        public required List<Venta> Ventas { get; set; }
    }
}