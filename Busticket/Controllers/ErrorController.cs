using Microsoft.AspNetCore.Mvc;
using Busticket.DTOs;
namespace Busticket.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/HttpStatus/{code}")]
        public IActionResult HttpStatus(int code)
        {
            if (code == 404)
                return View("~/Views/Shared/404.cshtml");

            if (code == 403)
                return View("~/Views/Shared/403.cshtml");

            return View("~/Views/Shared/Error.cshtml");
        }

        [Route("Error/Error500")]
        public IActionResult Error500()
        {
            return View("~/Views/Shared/500.cshtml");
        }
    }
}