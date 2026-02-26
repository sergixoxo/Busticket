using Busticket.Data;
using Busticket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Busticket.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        /* ===================== LOGIN ===================== */

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Ingrese correo y contraseña";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Error = "No existe una cuenta con este correo.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                false,
                false
            );

            if (!result.Succeeded)
            {
                ViewBag.Error = "La contraseña es incorrecta.";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        /* ===================== REGISTER ===================== */

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.TipoUsuario == "Empresa")
            {
                if (string.IsNullOrWhiteSpace(model.NombreEmpresa))
                    ModelState.AddModelError("NombreEmpresa", "El nombre de la empresa es obligatorio");

                if (string.IsNullOrWhiteSpace(model.Nit))
                    ModelState.AddModelError("Nit", "El NIT es obligatorio");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.Nombre))
                    ModelState.AddModelError("Nombre", "El nombre es obligatorio");
            }

            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ViewBag.Error = "Este correo ya está registrado";
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(" | ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            var role = model.TipoUsuario == "Empresa" ? "Empresa" : "Cliente";
            await _userManager.AddToRoleAsync(user, role);

            if (model.TipoUsuario == "Empresa")
            {
                var empresa = new Empresa
                {
                    Nombre = model.NombreEmpresa!,
                    Nit = model.Nit!,
                    Email = model.Email,
                    UserId = user.Id
                };

                _context.Empresa.Add(empresa);
            }
            else
            {
                // ✅ CREAR CLIENTE (ESTO TE FALTABA)
                var cliente = new Cliente
                {
                    UserId = user.Id,
                    Nombre = model.Nombre!,
                    Telefono = model.Telefono
                };

                _context.Cliente.Add(cliente);
            }

            await _context.SaveChangesAsync();


            // LOGIN AUTOMÁTICO
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        /* ===================== LOGOUT ===================== */

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
