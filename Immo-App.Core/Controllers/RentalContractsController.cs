using Immo_App.Core.Data;
using Immo_App.Core.Models.Apartment;
using Immo_App.Core.Models.RentalContract;
using Immo_App.Core.Models.Tenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Controllers
{
    public class RentalContractsController : Controller
    {
        private readonly ImmoDbContext immoDbContext;
        public RentalContractsController(ImmoDbContext immoDbContext)
        {
            this.immoDbContext = immoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rentalContracts = (from rc in immoDbContext.rental_contract
                                   join t in immoDbContext.tenant on rc.fk_tenant_id equals t.id
                                   join a in immoDbContext.apartment on rc.fk_apartment_id equals a.id
                                   select new IndexRentalContractViewModel
                                   {
                                       id = rc.id,
                                       tenant_name = t.first_name + " " + t.last_name,
                                       apartment_address = a.address + ", " + a.zip_code + " " + a.city
                                   }).OrderBy(rc => rc.id).ToList();

            return View("Index", rentalContracts);
        }
    }
}
