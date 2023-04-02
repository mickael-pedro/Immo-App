using Immo_App.Core.Data;
using Immo_App.Core.Models.RentalContract;
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
                                       apartment_address = a.address + (a.address_complement != null ? " " + a.address_complement : null) + ", " + a.zip_code + " " + a.city,
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

        public async Task<IActionResult> Delete(int id)
        {
            var rentalContract = await immoDbContext.rental_contract.FirstOrDefaultAsync(x => x.id == id);

            if (rentalContract != null)
            {
                immoDbContext.rental_contract.Remove(rentalContract);
                await immoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            var rentalContractData = await (from rc in immoDbContext.rental_contract
                                   where rc.id == id
                                   join t in immoDbContext.tenant on rc.fk_tenant_id equals t.id
                                   join a in immoDbContext.apartment on rc.fk_apartment_id equals a.id
                                   select new DetailRentalContractViewModel
                                   {
                                       id = rc.id,
                                       charges_price = rc.charges_price,
                                       rent_price = rc.rent_price,
                                       security_deposit_price = rc.security_deposit_price,
                                       security_deposit_status = rc.security_deposit_status,
                                       tenant_balance = rc.tenant_balance,
                                       rental_status = rc.rental_status,
                                       rental_active = rc.rental_active,
                                       tenant_name = t.civility + " " + t.first_name + " " + t.last_name,
                                       tenant_email = t.email,
                                       apartment_address = a.address + (a.address_complement != null ? " " + a.address_complement : null) + ", " + a.zip_code + " " + a.city,
                                   }).SingleAsync();

            rentalContractData.inventory_fixtures = await immoDbContext.inventory_fixture.Where(i => i.fk_rental_contract_id == rentalContractData.id).OrderBy(i => i.date_inv).ToListAsync();
            rentalContractData.invoices = await immoDbContext.invoice.Where(i => i.fk_rental_contract_id == rentalContractData.id).OrderByDescending(i => i.date_invoice).ToListAsync();
            rentalContractData.payments = await immoDbContext.payment.Where(p => p.fk_rental_contract_id == rentalContractData.id).OrderByDescending(p => p.date_payment).ToListAsync();

            return View(rentalContractData);
        }

        public async Task<IActionResult> EndContract(int id)
        {
            var rentalContract = await immoDbContext.rental_contract.FindAsync(id);

            if (rentalContract != null)
            {
                var inventory_fixtures = await immoDbContext.inventory_fixture.Where(i => i.fk_rental_contract_id == id).ToListAsync();

                if (rentalContract.rental_status == "En cours" && inventory_fixtures.Any(i => i.type == "Sortie"))
                {
                    rentalContract.rental_status = "Clôturé";
                    rentalContract.rental_active = false;

                    await immoDbContext.SaveChangesAsync();
                    return RedirectToAction("Detail", new { rentalContract.id });
                }
            }

            return RedirectToAction("Index");
        }
    }
}
