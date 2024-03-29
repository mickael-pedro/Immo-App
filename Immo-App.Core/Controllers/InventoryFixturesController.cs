﻿using Immo_App.Core.Data;
using Immo_App.Core.Helpers;
using Immo_App.Core.Models.InventoryFixture;
using Immo_App.Core.Models.Invoice;
using Immo_App.Core.Models.RentalContract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Controllers
{
	public class InventoryFixturesController : Controller
	{
        private readonly ImmoDbContext immoDbContext;
        public InventoryFixturesController(ImmoDbContext immoDbContext)
        {
            this.immoDbContext = immoDbContext;
        }

        [HttpGet("inventoryFixtures/Add/{rentalId:int}")]
        public IActionResult Add(int rentalId)
        {
            var data = new AddInventoryFixtureViewModel()
            {
                fk_rental_contract_id = rentalId
            };

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddInventoryFixtureViewModel addInventoryFixture)
        {
            var inventoryFixture = new InventoryFixture()
            {
                date_inv = addInventoryFixture.date_inv.ToUniversalTime(),
                type = addInventoryFixture.type,
                notes = addInventoryFixture.notes,
                fk_rental_contract_id = addInventoryFixture.fk_rental_contract_id
            };

            await immoDbContext.inventory_fixture.AddAsync(inventoryFixture);
            await immoDbContext.SaveChangesAsync();

            var helper = new UpdateStatusBalanceHelper(immoDbContext);
            await helper.UpdateRentalStatus(inventoryFixture.fk_rental_contract_id);

            return RedirectToAction("Detail", "rentalContracts", new { id = addInventoryFixture.fk_rental_contract_id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var inventoryFixture = await immoDbContext.inventory_fixture.FirstOrDefaultAsync(x => x.id == id);

            if (inventoryFixture != null)
            {
                return View(inventoryFixture);
            }

            return RedirectToAction("Index", "rentalContracts");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(InventoryFixture model)
        {
            var inventoryFixture = await immoDbContext.inventory_fixture.FindAsync(model.id);

            if (inventoryFixture != null)
            {
                inventoryFixture.id = model.id;
                inventoryFixture.date_inv = model.date_inv.ToUniversalTime();
                inventoryFixture.type = model.type;
                inventoryFixture.notes = model.notes;
                inventoryFixture.fk_rental_contract_id = model.fk_rental_contract_id;

                await immoDbContext.SaveChangesAsync();

                var helper = new UpdateStatusBalanceHelper(immoDbContext);
                await helper.UpdateRentalStatus(inventoryFixture.fk_rental_contract_id);

                return RedirectToAction("Detail", "rentalContracts", new { id = inventoryFixture.fk_rental_contract_id });
            }

            return RedirectToAction("Index", "rentalContracts");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var inventoryFixture = await immoDbContext.inventory_fixture.FirstOrDefaultAsync(x => x.id == id);

            if (inventoryFixture != null)
            {
                immoDbContext.inventory_fixture.Remove(inventoryFixture);
                await immoDbContext.SaveChangesAsync();

                var helper = new UpdateStatusBalanceHelper(immoDbContext);
                await helper.UpdateRentalStatus(inventoryFixture.fk_rental_contract_id);

                return RedirectToAction("Detail", "rentalContracts", new { id = inventoryFixture.fk_rental_contract_id });
            }

            return RedirectToAction("Index", "rentalContracts");
        }

        [HttpGet]
        public async Task<IActionResult> Notes(int id)
        {
            var inventoryFixture = await immoDbContext.inventory_fixture.FirstOrDefaultAsync(x => x.id == id);


            if (inventoryFixture != null)
            {
                var apartmentAddress = await (from rc in immoDbContext.rental_contract
                                                where rc.id == inventoryFixture.fk_rental_contract_id
                                                join a in immoDbContext.apartment on rc.fk_apartment_id equals a.id
                                                select a.address + (a.address_complement != null ? " " + a.address_complement : null) + ", " + a.zip_code + " " + a.city
                                                ).SingleAsync();

                var viewModel = new NotesInventoryFixtureViewModel()
                {
                    id = inventoryFixture.id,
                    date_inv = inventoryFixture.date_inv,
                    type = inventoryFixture.type,
                    notes = inventoryFixture.notes,
                    apartment_address = apartmentAddress
                };

                return View(viewModel);
            }

            return RedirectToAction("Index", "rentalContracts");
        }
    }
}
