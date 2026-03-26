namespace Busticket.DTOs
{
    public class CarritoItem
    {
        public int AsientoId { get; set; }
        public required string Codigo { get; set; }
        public decimal Precio { get; set; }
        public int RutaId { get; set; }
    }

    public class EmailServiceDTO
    {
        public int RutaId { get; set; }
        public required List<CarritoItem> Asientos { get; set; } = new();
    }
}