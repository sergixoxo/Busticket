using Busticket.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Busticket.DTOs;
namespace Busticket.Controllers
{
    public class PlanesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rutas = _context.Ruta
                .Include(r => r.Empresa)
                .Include(r => r.CiudadOrigen)
                .Include(r => r.CiudadDestino)
                .ToList();

            return View(rutas);
        }
    }
}
