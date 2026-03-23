using Busticket.Controllers;
using Busticket.Data;
using Busticket.Models;
using Busticket.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class PanelEmpresaControllerTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        return context;
    }

    private ClaimsPrincipal GetUser(string userId = "user123")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    private UserManager<IdentityUser> GetUserManagerMock(string userId = "user123")
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        var userManager = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );

        userManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                   .Returns(userId);

        return userManager.Object;
    }

 
    [Fact]
    public async Task Rutas_ReturnsOnlyEmpresaRoutes()
    {
        var context = GetDbContext();

        context.Ciudad.AddRange(
            new Ciudad { CiudadId = 1, Nombre = "A" },
            new Ciudad { CiudadId = 2, Nombre = "B" }
        );

        var empresa = new Empresa
        {
            EmpresaId = 1,
            UserId = "user123"
        };

        context.Empresa.Add(empresa);

        context.Ruta.AddRange(
            new Ruta { RutaId = 1, EmpresaId = 1, CiudadOrigenId = 1, CiudadDestinoId = 2 },
            new Ruta { RutaId = 2, EmpresaId = 99, CiudadOrigenId = 1, CiudadDestinoId = 2 }
        );

        await context.SaveChangesAsync();

        var controller = new PanelEmpresaController(context, GetUserManagerMock())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUser()
                }
            }
        };

        var result = await controller.Rutas() as ViewResult;
        var model = result.Model as List<Ruta>;

        Assert.Single(model);
    }

    // ============================
    // CREAR RUTA
    // ============================
    [Fact]
    public async Task CrearRuta_Valid_AddsRutaAndAsientos()
    {
        var context = GetDbContext();

        context.Empresa.Add(new Empresa
        {
            EmpresaId = 1,
            UserId = "user123"
        });

        await context.SaveChangesAsync();

        var controller = new PanelEmpresaController(context, GetUserManagerMock())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUser()
                }
            }
        };

        var ruta = new Ruta
        {
            CiudadOrigenId = 1,
            CiudadDestinoId = 2,
            Precio = 1000
        };

        controller.ModelState.Clear();

        var result = await controller.CrearRuta(ruta);

        Assert.Equal(1, context.Ruta.Count());
        Assert.Equal(20, context.Asiento.Count());
        Assert.IsType<RedirectToActionResult>(result);
    }

    // ============================
    // EDITAR RUTA
    // ============================
 
    // ============================
    // ELIMINAR RUTA NO EXISTE
    // ============================
    [Fact]
    public async Task EliminarRuta_NotFound_Redirects()
    {
        var context = GetDbContext();

        var controller = new PanelEmpresaController(context, GetUserManagerMock());

        var result = await controller.ConfirmarEliminarRuta(999) as RedirectToActionResult;

        Assert.Equal("Rutas", result.ActionName);
    }
}