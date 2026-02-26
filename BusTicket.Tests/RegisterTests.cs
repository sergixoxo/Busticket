using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Busticket.Controllers;
using Busticket.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Busticket.Tests
{
    public class RegistroTests
    {
        [Fact]
        public async Task Registro_DebeMostrarError_SiElModeloEsInvalido()
        {
            // 1. Arrange (Preparar)
            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            var controller = new AuthController(null, userManager.Object, null);

            // Simulamos que el usuario no llenó el email
            controller.ModelState.AddModelError("Email", "El campo es obligatorio");
            var modeloInvalido = new RegisterViewModel();

            // 2. Act (Actuar)
            var resultado = await controller.Register(modeloInvalido) as ViewResult;

            // 3. Assert (Verificar)
            Assert.False(controller.ModelState.IsValid);
            Assert.NotNull(resultado);
        }
    }
}