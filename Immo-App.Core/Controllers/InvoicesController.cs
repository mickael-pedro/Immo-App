using Immo_App.Core.Data;
using Immo_App.Core.Helpers;
using Immo_App.Core.Models.Invoice;
using Immo_App.Core.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Controllers
{
	public class InvoicesController : Controller
	{
        private readonly ImmoDbContext immoDbContext;
        public InvoicesController(ImmoDbContext immoDbContext)
        {
            this.immoDbContext = immoDbContext;
        }

        [HttpGet("invoices/Add/{rentalId:int}")]
        public IActionResult Add(int rentalId)
        {
            var data = new AddInvoiceViewModel()
            {
                fk_rental_contract_id = rentalId
            };

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddInvoiceViewModel addInvoice)
        {
            var invoice = new Invoice()
            {
                date_invoice = addInvoice.date_invoice.ToUniversalTime(),
                amount = addInvoice.amount,
                type = addInvoice.type,
                status = "Non payé",
                fk_rental_contract_id = addInvoice.fk_rental_contract_id
            };

            await immoDbContext.invoice.AddAsync(invoice);
            await immoDbContext.SaveChangesAsync();

            var helper = new UpdateStatusBalanceHelper(immoDbContext);
            await helper.UpdateBalance(invoice.fk_rental_contract_id);

            return RedirectToAction("Detail", "rentalContracts", new { id = addInvoice.fk_rental_contract_id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await immoDbContext.invoice.FirstOrDefaultAsync(x => x.id == id);

            if (invoice != null)
            {
                return View(invoice);
            }

            return RedirectToAction("Index", "rentalContracts");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Invoice model)
        {
            var invoice = await immoDbContext.invoice.FindAsync(model.id);

            if (invoice != null)
            {
                var oldInvoiceType = invoice.type;

                invoice.id = model.id;
                invoice.date_invoice = model.date_invoice.ToUniversalTime();
                invoice.amount = model.amount;
                invoice.type = model.type;
                invoice.status = model.status;
                invoice.fk_rental_contract_id = model.fk_rental_contract_id;

                await immoDbContext.SaveChangesAsync();

                var helper = new UpdateStatusBalanceHelper(immoDbContext);

                // If the invoice type changed it may have been for a security deposit
                // then update the rental status since it may not be paid anymore
                if (oldInvoiceType == "Dépôt de garantie")
                {
                    await helper.UpdateRentalStatus(invoice.fk_rental_contract_id);
                }

                await helper.UpdateInvoiceStatus(invoice.id);

                return RedirectToAction("Detail", "rentalContracts", new { id = invoice.fk_rental_contract_id });
            }

            return RedirectToAction("Index", "rentalContracts");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await immoDbContext.invoice.FirstOrDefaultAsync(x => x.id == id);

            if (invoice != null)
            {
                immoDbContext.invoice.Remove(invoice);
                await immoDbContext.SaveChangesAsync();

                var helper = new UpdateStatusBalanceHelper(immoDbContext);

                // If the invoice used to be for a security deposit, then update the rental status
                // since it may have changed the status
                if (invoice.type == "Dépôt de garantie")
                {
                    await helper.UpdateRentalStatus(invoice.fk_rental_contract_id);
                }

                await helper.UpdateBalance(invoice.fk_rental_contract_id);

                return RedirectToAction("Detail", "rentalContracts", new { id = invoice.fk_rental_contract_id });
            }

            return RedirectToAction("Index", "rentalContracts");
        }
    }
}
