namespace Busticket.Models // Ponlo en Models para que sea fácil de encontrar
{
    public class CarritoItem
    {
        public int AsientoId { get; set; }
        public required string Codigo { get; set; }
        public decimal Precio { get; set; }
        public int RutaId { get; set; }
    }
}