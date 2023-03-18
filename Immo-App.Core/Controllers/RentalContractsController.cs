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
                                       apartment_address = a.address + ", " + a.zip_code + " " + a.city,
                                       rental_status = rc.rental_status
                                   }).OrderBy(rc => rc.id).ToList();

            return View("Index", rentalContracts);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var data = new AddRentalContractViewModel
            {
                available_apartments = await (from a in immoDbContext.apartment
                                        where !immoDbContext.rental_contract.Any(r => a.id == r.fk_apartment_id && r.rental_active == true)
                                        select a).ToListAsync(),
                tenants = await immoDbContext.tenant.OrderBy(t => t.id).ToListAsync()
            };

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRentalContractViewModel addRentalContractRequest)
        {
            var rentalContract = new RentalContract()
            {
                charges_price = addRentalContractRequest.charges_price,
                rent_price = addRentalContractRequest.rent_price,
                security_deposit_price = addRentalContractRequest.security_deposit_price,
                security_deposit_status = "Non payé",
                tenant_balance = 0,
                rental_status = "En attente du paiement du dépôt de garantie",
                rental_active = true,
                fk_tenant_id = addRentalContractRequest.fk_tenant_id,
                fk_apartment_id = addRentalContractRequest.fk_apartment_id
            };

            await immoDbContext.rental_contract.AddAsync(rentalContract);
            await immoDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var rentalContract = await immoDbContext.rental_contract.FirstOrDefaultAsync(x => x.id == id);

            if (rentalContract != null)
            {
                var viewModel = new EditRentalContractViewModel()
                {
                    id = rentalContract.id,
                    charges_price = rentalContract.charges_price,
                    rent_price = rentalContract.rent_price,
                    security_deposit_price = rentalContract.security_deposit_price,
                    security_deposit_status = rentalContract.security_deposit_status,
                    tenant_balance = rentalContract.tenant_balance,
                    rental_status = rentalContract.rental_status,
                    rental_active = rentalContract.rental_active,
                    apartment = await immoDbContext.apartment.FirstOrDefaultAsync(a => a.id == rentalContract.fk_apartment_id),
                    tenant = await immoDbContext.tenant.FirstOrDefaultAsync(t => t.id == rentalContract.fk_tenant_id)
                };

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RentalContract model)
        {
            var rentalContract = await immoDbContext.rental_contract.FindAsync(model.id);

            if (rentalContract != null)
            {
                // We only change what the user should be able to change from the interface
                rentalContract.charges_price = model.charges_price;
                rentalContract.rent_price = model.rent_price;
                rentalContract.security_deposit_price = model.security_deposit_price;

                await immoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
