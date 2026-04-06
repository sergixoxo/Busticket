using Busticket.Data;
using Microsoft.AspNetCore.Mvc;
using Busticket.DTOs;
namespace Busticket.Controllers
{
    public class ItinerariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItinerariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // ✅ Error 500 simulado (se captura por UseExceptionHandler)
            throw new Exception("Error interno simulado en Itinerarios");
        }
    }
}
