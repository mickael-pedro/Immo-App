using Immo_App.Core.Data;
using Immo_App.Core.Models;
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

        [HttpPost]
        public async Task<IActionResult> Add(AddTenantViewModel addTenantRequest)
        {
            var tenant = new Tenant()
            {
                civility = addTenantRequest.civility,
                first_name = addTenantRequest.first_name,
                last_name = addTenantRequest.last_name,
                email = addTenantRequest.email
            };

            await immoDbContext.tenant.AddAsync(tenant);
            await immoDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
