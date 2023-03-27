using Immo_App.Core.Data;
using Immo_App.Core.Models.Invoice;
using Immo_App.Core.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Controllers
{
	public class PaymentsController : Controller
	{
        private readonly ImmoDbContext immoDbContext;
        public PaymentsController(ImmoDbContext immoDbContext)
        {
            this.immoDbContext = immoDbContext;
        }

        [HttpGet("payments/Add/{rentalId:int}")]
        public async Task<IActionResult> Add(int rentalId)
        {
            var data = new AddPaymentViewModel()
            {
                fk_rental_contract_id = rentalId,
                invoices = await immoDbContext.invoice.Where(i => i.fk_rental_contract_id == rentalId).OrderBy(i => i.id).ToListAsync()
            };

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPaymentViewModel addPayment)
        {
            var payment = new Payment()
            {
                date_payment = addPayment.date_payment.ToUniversalTime(),
                amount = addPayment.amount,
                origin = addPayment.origin,
                fk_invoice_id = addPayment.fk_invoice_id,
                fk_rental_contract_id = addPayment.fk_rental_contract_id
            };

            await immoDbContext.payment.AddAsync(payment);
            await immoDbContext.SaveChangesAsync();
            return RedirectToAction("Detail", "rentalContracts", new { id = addPayment.fk_rental_contract_id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var payment = await immoDbContext.payment.FirstOrDefaultAsync(x => x.id == id);

            if (payment != null)
            {
                var viewModel = new EditPaymentViewModel()
                {
                    id = payment.id,
                    date_payment = payment.date_payment.ToUniversalTime(),
                    amount = payment.amount,
                    origin = payment.origin,
                    fk_invoice_id = payment.fk_invoice_id,
                    fk_rental_contract_id = payment.fk_rental_contract_id,
                    invoices = await immoDbContext.invoice.Where(i => i.fk_rental_contract_id == payment.fk_rental_contract_id).OrderBy(i => i.id).ToListAsync()
                };

                return View(viewModel);
            }

            return RedirectToAction("Index", "rentalContracts");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPaymentViewModel model)
        {
            var payment = await immoDbContext.payment.FindAsync(model.id);

            if (payment != null)
            {
                payment.id = model.id;
                payment.date_payment = model.date_payment.ToUniversalTime();
                payment.amount = model.amount;
                payment.origin = model.origin;
                payment.fk_invoice_id = model.fk_invoice_id;
                payment.fk_rental_contract_id = model.fk_rental_contract_id;

                await immoDbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "rentalContracts", new { id = payment.fk_rental_contract_id });
            }

            return RedirectToAction("Index", "rentalContracts");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var payment = await immoDbContext.payment.FirstOrDefaultAsync(x => x.id == id);

            if (payment != null)
            {
                immoDbContext.payment.Remove(payment);
                await immoDbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "rentalContracts", new { id = payment.fk_rental_contract_id });
            }

            return RedirectToAction("Index", "rentalContracts");
        }
    }
}
