using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Busticket.Data;
using Microsoft.AspNetCore.Identity;

[Authorize]
public class AsientosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AsientosController(ApplicationDbContext context,
                              UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // 🔒 RESERVAR ASIENTO (TEMPORAL)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reservar([FromBody] int asientoId)
    {
        var user = await _userManager.GetUserAsync(User);

        var asiento = await _context.Asiento
            .FirstOrDefaultAsync(a => a.AsientoId == asientoId);

        if (asiento == null)
            return NotFound();

        // ❌ Ya vendido
        if (!asiento.Disponible)
            return BadRequest("Asiento vendido");

        // ❌ Reservado por otro
        if (asiento.Reservado && asiento.ReservadoPorUserId != user!.Id)
            return BadRequest("Asiento reservado");

        // ✔️ Reservar
        asiento.Reservado = true;
        asiento.ReservadoPorUserId = user!.Id;
        asiento.FechaReserva = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok();
    }

    // 🔓 LIBERAR ASIENTO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Liberar([FromBody] int asientoId)
    {
        var user = await _userManager.GetUserAsync(User);

        var asiento = await _context.Asiento
            .FirstOrDefaultAsync(a => a.AsientoId == asientoId
                                   && a.ReservadoPorUserId == user!.Id);

        if (asiento == null)
            return NotFound();

        asiento.Reservado = false;
        asiento.ReservadoPorUserId = null;
        asiento.FechaReserva = null;

        await _context.SaveChangesAsync();

        return Ok();
    }
}