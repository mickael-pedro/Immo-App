using Immo_App.Core.Data;
using Immo_App.Core.Models.Invoice;
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
                invoice.id = model.id;
                invoice.date_invoice = model.date_invoice.ToUniversalTime();
                invoice.amount = model.amount;
                invoice.type = model.type;
                invoice.status = model.status;
                invoice.fk_rental_contract_id = model.fk_rental_contract_id;

                await immoDbContext.SaveChangesAsync();

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

                return RedirectToAction("Detail", "rentalContracts", new { id = invoice.fk_rental_contract_id });
            }

            return RedirectToAction("Index", "rentalContracts");
        }
    }
}
