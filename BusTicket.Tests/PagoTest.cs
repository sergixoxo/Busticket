using Busticket.Controllers;
using Busticket.Data;
using Busticket.Models;
using Busticket.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;
using Busticket.DTOs;

namespace Busticket.Tests.Controllers
{
    public class PagoControllerTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w =>
                    w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning)
                )
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }

        private class FakeSession : ISession
        {
            private readonly Dictionary<string, byte[]> _store = new();
            public string Id => Guid.NewGuid().ToString();
            public bool IsAvailable => true;
            public IEnumerable<string> Keys => _store.Keys;
            public void Clear() => _store.Clear();
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => _store.Remove(key);
            public void Set(string key, byte[] value) => _store[key] = value;
            public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value);
        }

        private static PagoController CreateController(ApplicationDbContext context, string userId = "user-test")
        {
            var controller = new PagoController(context);
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
                    "TestAuth"
                )
            );

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    Session = new FakeSession()
                }
            };

            return controller;
        }

        [Fact]
        public void Index_SinCarrito_RedireccionaHome()
        {
            var context = CreateDbContext();
            var controller = CreateController(context);
            var result = controller.Index();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task FinalizarPago_ModeloNull_Redirecciona()
        {
            var context = CreateDbContext();
            var controller = CreateController(context);
            var result = await controller.FinalizarPago(null);
            Assert.IsType<RedirectToActionResult>(result);
        }

        // ... (mismo código inicial de configuración)

        [Fact]
        public async Task FinalizarPago_Valido_CreaVentaBoletosYActualizaAsientos()
        {
            var context = CreateDbContext();

            var empresa = new Empresa
            {
                EmpresaId = 1,
                Nombre = "Empresa Test",
                Nit = "123456-7"
            };

            var ruta = new Ruta
            {
                RutaId = 1,
                EmpresaId = 1,
                Empresa = empresa,
                Precio = 25000,
                CiudadOrigen = new Ciudad { Nombre = "Origen" },
                CiudadDestino = new Ciudad { Nombre = "Destino" }
            };

            var user = new IdentityUser
            {
                Id = "user-test",
                UserName = "usuario-test",
                Email = "test@test.com"
            };

            var asiento = new Asiento
            {
                AsientoId = 1,
                RutaId = 1,
                Disponible = true,
                Numero = 1,        // ✅ Requerido según tu modelo
                Codigo = "A1"      // ✅ Ahora compilará tras actualizar Asiento.cs
            };

            context.Empresa.Add(empresa);
            context.Ruta.Add(ruta);
            context.Users.Add(user);
            context.Asiento.Add(asiento);
            context.SaveChanges();

            var controller = CreateController(context, user.Id);

            // 🔹 ViewModel: Se añaden todos los campos 'required' que causaban error CS9035
            var model = new PagoViewModel
            {
                RutaId = 1,
                Total = 25000,
                Asientos = new List<string> { "1" },
                Nombre = "Test User",
                NumeroTarjeta = "1234123412341234",
                CVC = "123",
                Validez = "12/26",
                // ✅ Campos adicionales detectados en el error CS9035 de la captura:
                Origen = "Origen Test",
                Destino = "Destino Test",
                Empresa = "Empresa Test",
                Duracion = "2h",
                Fecha = "2026-05-20",
                Hora = "14:00",
                Descuento = ""
            };

            var result = await controller.FinalizarPago(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ConfirmacionPago", redirect.ActionName);
            Assert.Single(context.Venta);
            var asientoDb = context.Asiento.First();
            Assert.False(asientoDb.Disponible);
        }

        [Fact]
        public async Task ConfirmacionPago_NoExiste_RedireccionaHome()
        {
            var context = CreateDbContext();
            var controller = CreateController(context);
            var result = await controller.ConfirmacionPago(99);
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}