using Busticket.Data;
using Busticket.DTOs;
using Busticket.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Busticket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SkiaSharp;
using QuestPDF.Drawing;
using QRCoder;



namespace Busticket.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: /Pago
        // =========================
        public IActionResult Index()
        {
            var carrito = HttpContext.Session
      .GetObjectFromJson<List<CarritoItem>>("Carrito")
      ?? new List<CarritoItem>();

            if (!carrito.Any())
                return RedirectToAction("Index", "Home");

            var total = carrito.Sum(c => c.Precio);

            var primerAsiento = _context.Asiento
                .Include(a => a.Ruta).ThenInclude(r => r.CiudadOrigen)
                .Include(a => a.Ruta).ThenInclude(r => r.CiudadDestino)
                .Include(a => a.Ruta).ThenInclude(r => r.Empresa)
                .FirstOrDefault(a => a.AsientoId == carrito.First().AsientoId);

            if (primerAsiento == null)
                return RedirectToAction("Index", "Home");

            return View(new PagoViewModel
            {
                RutaId = primerAsiento.RutaId,
                Asientos = carrito.Select(c => c.AsientoId.ToString()).ToList(),
                Total = total,
                Origen = primerAsiento.Ruta.CiudadOrigen.Nombre,
                Destino = primerAsiento.Ruta.CiudadDestino.Nombre,
                Empresa = primerAsiento.Ruta.Empresa.Nombre,
                Duracion = primerAsiento.Ruta.DuracionMin + " min",
                Fecha = DateTime.Now.ToString("dd/MM/yyyy"),
                Hora = "00:00"
            });

        }


        // =========================
        // POST: /Pago/FinalizarPago
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> FinalizarPago(PagoViewModel model)
        {
            // 🔹 Validaciones básicas
            if (model == null || model.Asientos == null || !model.Asientos.Any())
                return RedirectToAction("Index");

            if (model.Total <= 0)
                return RedirectToAction("Index");

            // 🔹 Usuario autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            // 🔹 Convertir asientos string → int (CORRECTO)
            var asientosIds = model.Asientos
                .Select(a => int.TryParse(a, out int id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            if (!asientosIds.Any())
                return RedirectToAction("Index");

            // 🔹 Obtener asientos válidos y disponibles
            var asientosDb = await _context.Asiento
                .Where(a =>
                    asientosIds.Contains(a.AsientoId) &&
                    a.RutaId == model.RutaId &&
                    a.Disponible)
                .ToListAsync();

            if (!asientosDb.Any())
                return RedirectToAction("Index");

            // 🔹 Obtener ruta
            var ruta = await _context.Ruta.FirstOrDefaultAsync(r => r.RutaId == model.RutaId);
            if (ruta == null)
                return RedirectToAction("Index");

            Venta venta;

            // =========================
            // 🔒 TRANSACCIÓN
            // =========================
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Crear venta
                venta = new Venta
                {
                    Fecha = DateTime.Now,
                    Total = model.Total,
                    RutaId = ruta.RutaId,
                    EmpresaId = ruta.EmpresaId,
                    UserId = userId
                };

                _context.Venta.Add(venta);
                await _context.SaveChangesAsync();

                // 2️⃣ ASIENTOS → VENDIDOS
                foreach (var asiento in asientosDb)
                {
                    asiento.Disponible = false;
                    asiento.Reservado = false;
                    asiento.ReservadoPorUserId = null;
                    asiento.FechaReserva = null;
                }

                // 3️⃣ Crear boletos
                foreach (var asiento in asientosDb)
                {
                    _context.Boleto.Add(new Boleto
                    {
                        VentaId = venta.VentaId,
                        UserId = userId,
                        RutaId = ruta.RutaId,
                        AsientoId = asiento.AsientoId,
                        Precio = ruta.Precio,
                        FechaCompra = DateTime.Now,
                        Codigo = $"BT-{Guid.NewGuid():N}".Substring(0, 10).ToUpper()
                    });
                }

                // 4️⃣ Guardar TODO
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            // 🧹 Limpiar carrito
            HttpContext.Session.Remove("Carrito");
            HttpContext.Session.Remove("Total");

            // ✅ REDIRECCIÓN CORRECTA
            return RedirectToAction(
                "ConfirmacionPago",
                "Pago",
                new { ventaId = venta.VentaId }
            );
        }

    



        // =========================
        // GET: /Pago/ConfirmacionPago
        // =========================
        public async Task<IActionResult> ConfirmacionPago(int ventaId)
        {
            var venta = await _context.Venta
                .Include(v => v.Boletos)
                .ThenInclude(b => b.Asiento)
                .FirstOrDefaultAsync(v => v.VentaId == ventaId);

            if (venta == null)
                return RedirectToAction("Index", "Home");

            return View(venta);
        }

        // =========================
        // GET: /Pago/DescargarBoleto
        // =========================
        public async Task<IActionResult> DescargarBoleto(int ventaId)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var venta = await _context.Venta
                .Include(v => v.Ruta).ThenInclude(r => r.Empresa)
                .Include(v => v.Boletos).ThenInclude(b => b.Asiento)
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.VentaId == ventaId);

            if (venta == null)
                return NotFound();

            // 🔹 Links aleatorios
            var instagramLinks = new[]
            {
        "https://www.instagram.com/itsadanba",
        "https://www.instagram.com/sergixoxo",
        "https://www.instagram.com/jacomu_ssi"
    };

            var instagramRandom = instagramLinks[new Random().Next(instagramLinks.Length)];

            // 🔹 Generar QR
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(instagramRandom, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrData);
            byte[] qrBytes = qrCode.GetGraphic(20);

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(250, 600); // 🎫 FORMATO TICKET
                    page.Margin(20);

                    page.Content().Column(col =>
                    {
                        col.Spacing(8);

                        // HEADER
                        col.Item().AlignCenter().Text("BUSTICKET")
                            .FontSize(20)
                            .Bold();

                        col.Item().AlignCenter().Text("Boleto de Viaje")
                            .FontSize(12)
                            .Italic();

                        col.Item().LineHorizontal(1);

                        // INFO
                        col.Item().Text($"Empresa: {venta.Ruta.Empresa.Nombre}");
                        col.Item().Text($"Cliente: {venta.User.UserName}");
                        col.Item().Text($"Fecha: {venta.Fecha:dd/MM/yyyy}");

                        col.Item().LineHorizontal(1);

                        col.Item().Text("Asientos:")
                            .Bold();

                        foreach (var b in venta.Boletos)
                        {
                            col.Item().Text($"• Asiento {b.Asiento.Numero}");
                        }

                        col.Item().LineHorizontal(1);

                        col.Item().AlignCenter()
                            .Text($"TOTAL: {venta.Total:N0} COP")
                            .FontSize(14)
                            .Bold();

                        col.Item().PaddingTop(10)
                            .AlignCenter()
                            .Text("Escanea el QR")
                            .FontSize(10);

                        // QR ✅ CORRECTO
                        col.Item()
                            .AlignCenter()
                            .Width(120)
                            .Height(120)
                            .Image(qrBytes);

                        col.Item().PaddingTop(10)
                            .AlignCenter()
                            .Text("Gracias por viajar con nosotros")
                            .FontSize(9)
                            .Italic();
                    });
                });
            });

            var bytes = pdf.GeneratePdf();
            return File(bytes, "application/pdf", $"Boleto_{ventaId}.pdf");
        }
    }
}
