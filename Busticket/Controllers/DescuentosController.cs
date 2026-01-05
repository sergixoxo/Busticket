using Busticket.Data;
using Busticket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Busticket.Controllers
{
    public class DescuentosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static Random rnd = new Random();

        public DescuentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ofertas = _context.Oferta.ToList();
            return View(ofertas);
        }
    }
}