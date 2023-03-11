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
            var tenants = await immoDbContext.tenant.OrderBy(t => t.id).ToListAsync();
            return View("Index", tenants);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
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

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateTenantViewModel model)
        {
            var tenant = await immoDbContext.tenant.FindAsync(model.id);

            if (tenant != null)
            {
                tenant.id = model.id;
                tenant.civility = model.civility;
                tenant.first_name = model.first_name;
                tenant.last_name = model.last_name;
                tenant.email = model.email;

                await immoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
