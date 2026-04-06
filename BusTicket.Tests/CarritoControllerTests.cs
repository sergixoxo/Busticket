using Busticket.Controllers;
using Busticket.Data;
using Busticket.DTOs;
using Busticket.Models; // ✅ Aseguramos el acceso a los modelos
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

public class CarritoControllerTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ✅ ID único para evitar conflictos
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }

    private ISession GetSession()
    {
        return new TestSession(); // Asegúrate de que TestSession esté definida en tu proyecto de pruebas
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

        // ✅ Añadimos propiedades requeridas por el modelo Ruta
        context.Ruta.Add(new Ruta
        {
            RutaId = 1,
            CiudadOrigen = new Ciudad { Nombre = "A" },
            CiudadDestino = new Ciudad { Nombre = "B" },
            Empresa = new Empresa { Nombre = "Test", Nit = "123" }
        });
        await context.SaveChangesAsync();

        var controller = GetController(context);

        // ✅ Corregido a EmailServiceDTO
        var dto = new EmailServiceDTO
        {
            RutaId = 1,
            Asientos = new List<CarritoItem>
            {
                // ✅ Añadido 'Codigo' que es required
                new CarritoItem { AsientoId = 1, Precio = 1000, Codigo = "A1" }
            }
        };

        var result = await controller.Agregar(dto) as OkObjectResult;

        Assert.NotNull(result);
    }

    // ============================
    // AGREGAR SIN ASIENTOS
    // ============================
    [Fact]
    public async Task Agregar_NoSeats_ReturnsBadRequest()
    {
        var context = GetDbContext();
        var controller = GetController(context);

        // ✅ Corregido a EmailServiceDTO
        var dto = new EmailServiceDTO
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

        // ✅ Corregido a EmailServiceDTO
        var dto = new EmailServiceDTO
        {
            RutaId = 999,
            Asientos = new List<CarritoItem>
            {
                new CarritoItem { AsientoId = 1, Precio = 1000, Codigo = "A1" }
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
            new CarritoItem { AsientoId = 1, Precio = 1000, Codigo = "A1" }
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