using Microsoft.AspNetCore.Mvc;

namespace Immo_App.Core.Controllers
{
    public class TenantsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("Index");
        }
    }
}
