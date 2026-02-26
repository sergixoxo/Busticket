using Busticket.Data;
using Busticket.Models;
using Busticket.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Busticket.Controllers
{
    [Authorize(Roles = "Empresa,Admin")]
    public class PanelEmpresaController : Controller


    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PanelEmpresaController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // DASHBOARD
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(e => e.UserId == userId);

            var ventas = await _context.Venta
                .Where(v => v.EmpresaId == empresa.EmpresaId)
                .Include(v => v.User)
                .Include(v => v.Empresa)
                .ToListAsync();

            return View(new PanelEmpresaVM
            {
                Empresa = empresa,
                Ventas = ventas
            });
        }

        // LISTAR RUTAS
        public async Task<IActionResult> Rutas()
        {
            var userId = _userManager.GetUserId(User);

            var empresa = await _context.Empresa
      .FirstOrDefaultAsync(e => e.UserId == userId);

            if (empresa == null)
            {
                TempData["ErrorMessage"] = "No existe una empresa asociada a este usuario.";
                return RedirectToAction("Index", "Home");
            }
            var rutas = await _context.Ruta
                .Where(r => r.EmpresaId == empresa.EmpresaId)
                .Include(r => r.CiudadOrigen)
                .Include(r => r.CiudadDestino)
                .ToListAsync();

            return View(rutas);
        }

        // CREAR RUTA GET
        public IActionResult CrearRuta()
        {
            ViewBag.Ciudades = _context.Ciudad.ToList();
            return View(new Ruta());
        }

        // CREAR RUTA POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRuta(Ruta ruta)
        {
            ModelState.Remove("EmpresaId");

            if (ruta.CiudadOrigenId == ruta.CiudadDestinoId)
                ModelState.AddModelError("", "Origen y destino no pueden ser iguales");

            if (!ModelState.IsValid)
            {
                ViewBag.Ciudades = _context.Ciudad.ToList();
                return View(ruta);
            }

            var userId = _userManager.GetUserId(User);

            var empresa = await _context.Empresa
                .FirstAsync(e => e.UserId == userId);

            ruta.EmpresaId = empresa.EmpresaId;

            _context.Ruta.Add(ruta);
            await _context.SaveChangesAsync();

            var asientos = Enumerable.Range(1, 20)
                .Select(i => new Asiento { Numero = i, RutaId = ruta.RutaId });

            _context.Asiento.AddRange(asientos);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Rutas));
        }
    }
}