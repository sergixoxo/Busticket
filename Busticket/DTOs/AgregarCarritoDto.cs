public class CarritoItem
{
    public int AsientoId { get; set; }
    public required string Codigo { get; set; }  // ✅ required
    public decimal Precio { get; set; }
    public int RutaId { get; set; }
}