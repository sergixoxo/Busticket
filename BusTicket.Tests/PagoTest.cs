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

            public Task CommitAsync(CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            public Task LoadAsync(CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            public void Remove(string key) => _store.Remove(key);

            public void Set(string key, byte[] value) => _store[key] = value;

            public bool TryGetValue(string key, out byte[] value)
                => _store.TryGetValue(key, out value);
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

   
        [Fact]
        public async Task FinalizarPago_Valido_CreaVentaBoletosYActualizaAsientos()
        {
            var context = CreateDbContext();

            // 🔹 Empresa
            var empresa = new Empresa
            {
                EmpresaId = 1,
                Nombre = "Empresa Test"
            };

            // 🔹 Ruta
            var ruta = new Ruta
            {
                RutaId = 1,
                EmpresaId = 1,
                Empresa = empresa,
                Precio = 25000
            };

            // 🔹 Usuario
            var user = new IdentityUser
            {
                Id = "user-test",
                UserName = "usuario-test"
            };

            // 🔹 Asiento
            var asiento = new Asiento
            {
                AsientoId = 1,
                RutaId = 1,
                Disponible = true
            };

            context.Empresa.Add(empresa);
            context.Ruta.Add(ruta);
            context.Users.Add(user);
            context.Asiento.Add(asiento);
            context.SaveChanges();

            var controller = CreateController(context, user.Id);

            var model = new PagoViewModel
            {
                RutaId = 1,
                Total = 25000,
                Asientos = new List<string> { "1" }
            };

            // 🔹 Act
            var result = await controller.FinalizarPago(model);

            // 🔹 Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ConfirmacionPago", redirect.ActionName);

            Assert.Single(context.Venta);
            Assert.Single(context.Boleto);

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