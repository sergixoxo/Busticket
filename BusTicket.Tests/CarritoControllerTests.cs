using Busticket.Controllers;
using Busticket.Data;
using Busticket.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

public class CarritoControllerTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "CarritoTestDB")
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }

    // 🔥 Fake Session
    private ISession GetSession()
    {
        var session = new TestSession();
        return session;
    }

    private CarritoController GetController(ApplicationDbContext context)
    {
        var controller = new CarritoController(context);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = GetSession();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }

    // ============================
    // AGREGAR OK
    // ============================
    [Fact]
    public async Task Agregar_AddsItemsToCart()
    {
        var context = GetDbContext();

        context.Ruta.Add(new Busticket.Models.Ruta { RutaId = 1 });
        await context.SaveChangesAsync();

        var controller = GetController(context);

        var dto = new EmailService
        {
            RutaId = 1,
            Asientos = new List<CarritoItem>
            {
                new CarritoItem { AsientoId = 1, Precio = 1000 }
            }
        };

        var result = await controller.Agregar(dto) as OkObjectResult;

        Assert.NotNull(result);

        var data = result.Value;
        Assert.NotNull(data);
    }

    // ============================
    // AGREGAR SIN ASIENTOS
    // ============================
    [Fact]
    public async Task Agregar_NoSeats_ReturnsBadRequest()
    {
        var context = GetDbContext();
        var controller = GetController(context);

        var dto = new EmailService
        {
            RutaId = 1,
            Asientos = new List<CarritoItem>()
        };

        var result = await controller.Agregar(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // ============================
    // AGREGAR RUTA NO EXISTE
    // ============================
    [Fact]
    public async Task Agregar_RutaNotFound_ReturnsBadRequest()
    {
        var context = GetDbContext();
        var controller = GetController(context);

        var dto = new EmailService
        {
            RutaId = 999,
            Asientos = new List<CarritoItem>
            {
                new CarritoItem { AsientoId = 1, Precio = 1000 }
            }
        };

        var result = await controller.Agregar(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // ============================
    // INDEX (GET)
    // ============================
    [Fact]
    public void Index_ReturnsCart()
    {
        var context = GetDbContext();
        var controller = GetController(context);

        var result = controller.Index() as ViewResult;

        Assert.NotNull(result);
    }

    // ============================
    // ELIMINAR
    // ============================
    [Fact]
    public void Eliminar_RemovesItem()
    {
        var context = GetDbContext();
        var controller = GetController(context);

        var carrito = new List<CarritoItem>
        {
            new CarritoItem { AsientoId = 1, Precio = 1000 }
        };

        controller.HttpContext.Session.SetString("Carrito",
            JsonSerializer.Serialize(carrito));

        var request = new CarritoController.EliminarAsientoRequest
        {
            AsientoId = 1
        };

        var result = controller.Eliminar(request) as OkObjectResult;

        Assert.NotNull(result);
    }
}