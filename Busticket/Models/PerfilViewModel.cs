using System;
using System.Collections.Generic;

namespace Busticket.Models
{
    public class PerfilViewModel
    {
        // 👤 Datos del cliente
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;

        // 🖼️ Foto
        public string FotoUrl { get; set; } = "/img/default-user.png";

        // 🚌 Historial de viajes
        public List<ViajeViewModel> HistorialViajes { get; set; } = new();
    }

    public class ViajeViewModel
    {
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}