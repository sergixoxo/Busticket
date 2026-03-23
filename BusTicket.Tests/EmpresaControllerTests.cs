using Xunit;
using Microsoft.EntityFrameworkCore;
using Busticket.Controllers;
using Busticket.Data;
using Busticket.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

public class EmpresaControllerTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;

        var context = new ApplicationDbContext(options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

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

    // ============================
    // INDEX
    // ============================
    [Fact]
    public async Task Index_ReturnsOnlyUserEmpresas()
    {
        var context = GetDbContext();

        context.Empresa.AddRange(
            new Empresa { Nombre = "Empresa1", UserId = "user123" },
            new Empresa { Nombre = "Empresa2", UserId = "otroUser" }
        );

        await context.SaveChangesAsync();

        var controller = new EmpresaController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = GetUser("user123") }
        };

        var result = await controller.Index() as ViewResult;
        var model = result.Model as List<Empresa>;

        Assert.Single(model);
        Assert.Equal("Empresa1", model.First().Nombre);
    }

    // ============================
    // CREAR
    // ============================
    [Fact]
    public async Task Crear_Post_Valid_AddsEmpresa()
    {
        var context = GetDbContext();

        var controller = new EmpresaController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = GetUser() }
        };

        var empresa = new Empresa { Nombre = "Nueva Empresa" };

        var result = await controller.Crear(empresa);

        Assert.Equal(1, context.Empresa.Count());
        Assert.IsType<RedirectToActionResult>(result);
    }

    // ============================
    // EDITAR GET
    // ============================
    [Fact]
    public async Task Editar_Get_ReturnsEmpresa()
    {
        var context = GetDbContext();

        var empresa = new Empresa { EmpresaId = 1, Nombre = "Test", UserId = "user123" };
        context.Empresa.Add(empresa);
        await context.SaveChangesAsync();

        var controller = new EmpresaController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = GetUser() }
        };

        var result = await controller.Editar(1) as ViewResult;

        Assert.NotNull(result);
        Assert.IsType<Empresa>(result.Model);
    }

    // ============================
    // EDITAR POST
    // ============================
    [Fact]
    public async Task Editar_Post_UpdatesEmpresa()
    {
        var context = GetDbContext();

        var empresa = new Empresa { EmpresaId = 1, Nombre = "Viejo", UserId = "user123" };
        context.Empresa.Add(empresa);
        await context.SaveChangesAsync();

        var controller = new EmpresaController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = GetUser() }
        };

        empresa.Nombre = "Nuevo";

        var result = await controller.Editar(empresa);

        Assert.Equal("Nuevo", context.Empresa.First().Nombre);
        Assert.IsType<RedirectToActionResult>(result);
    }

    // ============================
    // ELIMINAR
    // ============================
    [Fact]
    public async Task EliminarConfirmado_DeletesEmpresa()
    {
        var context = GetDbContext();

        var empresa = new Empresa { EmpresaId = 1, Nombre = "Eliminar", UserId = "user123" };
        context.Empresa.Add(empresa);
        await context.SaveChangesAsync();

        var controller = new EmpresaController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = GetUser() }
        };

        var result = await controller.EliminarConfirmado(1);

        Assert.Empty(context.Empresa);
        Assert.IsType<RedirectToActionResult>(result);
    }
}