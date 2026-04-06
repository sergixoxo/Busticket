using Busticket.Data;
using Busticket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Busticket.Controllers
{
    [Authorize(Roles = "Empresa,Admin")]
    public class EmpresaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmpresaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // LISTAR (SOLO LA EMPRESA DEL USUARIO)
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var empresas = await _context.Empresa
                .Where(e => e.UserId == userId)
                .ToListAsync();

            return View(empresas);
        }

        // ===============================
        // CREAR
        // ===============================
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Empresa());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Empresa empresa)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (!ModelState.IsValid)
                return View(empresa);

            empresa.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            empresa.FechaRegistro = DateTime.Now;

            _context.Empresa.Add(empresa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // ===============================
        // EDITAR
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(e => e.EmpresaId == id && e.UserId == userId);

            if (empresa == null)
                return NotFound();

            return View(empresa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Empresa empresa)
        {
            if (!ModelState.IsValid)
                return View(empresa);

            empresa.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Empresa.Update(empresa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // ELIMINAR
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(e => e.EmpresaId == id && e.UserId == userId);

            if (empresa == null)
                return NotFound();

            return View(empresa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(e => e.EmpresaId == id && e.UserId == userId);

            if (empresa == null)
                return NotFound();

            _context.Empresa.Remove(empresa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}