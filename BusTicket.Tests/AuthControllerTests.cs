using Busticket.Controllers;
using Busticket.Data;
using Busticket.Models;
using Busticket.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Busticket.Tests.Controllers
{
  public class AuthControllerTests
  {
    private readonly Mock<UserManager<IdentityUser>> _userManager;
    private readonly Mock<SignInManager<IdentityUser>> _signInManager;

    public AuthControllerTests()
    {
      var userStore = new Mock<IUserStore<IdentityUser>>();

      _userManager = new Mock<UserManager<IdentityUser>>(
          userStore.Object, null, null, null, null, null, null, null, null);

      var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
      var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

      _signInManager = new Mock<SignInManager<IdentityUser>>(
          _userManager.Object,
          contextAccessor.Object,
          userPrincipalFactory.Object,
          null, null, null, null);
    }

    // 🔥 DB EN MEMORIA (igual que pagos)
    private ApplicationDbContext CreateDbContext()
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(Guid.NewGuid().ToString())
          .Options;

      return new ApplicationDbContext(options);
    }

    // =========================
    // ❌ PASSWORD INVÁLIDA
    // =========================
    [Fact]
    public async Task Register_PasswordSinNumero_DeberiaFallar()
    {
      var context = CreateDbContext();

      var model = new RegisterViewModel
      {
        Email = "test@test.com",
        Password = "Password@", // ❌ sin número
        TipoUsuario = "Cliente",
        Nombre = "Sergio"
      };

      _userManager
          .Setup(x => x.FindByEmailAsync(model.Email))
          .ReturnsAsync((IdentityUser)null);

      _userManager
          .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), model.Password))
          .ReturnsAsync(IdentityResult.Failed(
              new IdentityError { Code = "PasswordRequiresDigit" }
          ));

      var controller = new AuthController(
          _signInManager.Object,
          _userManager.Object,
          context
      );

      var result = await controller.Register(model);

      var view = Assert.IsType<ViewResult>(result);

      Assert.False(controller.ModelState.IsValid);
      Assert.True(controller.ModelState.ContainsKey("Password"));
    }

    // =========================
    // ✅ PASSWORD VÁLIDA
    // =========================
    [Fact]
    public async Task Register_PasswordValida_DeberiaRedirigir()
    {
      var context = CreateDbContext();

      var model = new RegisterViewModel
      {
        Email = "test@test.com",
        Password = "Password1@", // ✅ válida
        TipoUsuario = "Cliente",
        Nombre = "Sergio"
      };

      _userManager
          .Setup(x => x.FindByEmailAsync(model.Email))
          .ReturnsAsync((IdentityUser)null);

      _userManager
          .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), model.Password))
          .ReturnsAsync(IdentityResult.Success);

      _userManager
          .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
          .ReturnsAsync(IdentityResult.Success);

      _signInManager
          .Setup(x => x.SignInAsync(It.IsAny<IdentityUser>(), false, null))
          .Returns(Task.CompletedTask);

      var controller = new AuthController(
          _signInManager.Object,
          _userManager.Object,
          context
      );

      var result = await controller.Register(model);

      var redirect = Assert.IsType<RedirectToActionResult>(result);

      Assert.Equal("Index", redirect.ActionName);
      Assert.Equal("Home", redirect.ControllerName);
    }
  }
}
