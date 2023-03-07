using Immo_App.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Controllers
{
    public class TenantsController : Controller
    {
        private readonly ImmoDbContext immoDbContext;
        public TenantsController(ImmoDbContext immoDbContext)
        {
            this.immoDbContext = immoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tenants = await immoDbContext.tenant.ToListAsync();
            return View("Index", tenants);
        }
    }
}
