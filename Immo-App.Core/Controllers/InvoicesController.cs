using Immo_App.Core.Data;
using Immo_App.Core.Helpers;
using Immo_App.Core.Models.Invoice;
using Immo_App.Core.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
        public async Task<IActionResult> Add(int rentalId)
        {
            var rentalContractData = await immoDbContext.rental_contract.FindAsync(rentalId);

            var data = new AddInvoiceViewModel()
            {
                fk_rental_contract_id = rentalId,
                rent_charges_sum = rentalContractData.rent_price + rentalContractData.charges_price,
                deposit_price = rentalContractData.security_deposit_price
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
                var rentalContractData = await immoDbContext.rental_contract.FindAsync(invoice.fk_rental_contract_id);

                var viewModel = new EditInvoiceViewModel()
                {
                    id = invoice.id,
                    date_invoice = invoice.date_invoice,
                    amount = invoice.amount,
                    type = invoice.type,
                    status = invoice.status,
                    fk_rental_contract_id = invoice.fk_rental_contract_id,
                    rent_charges_sum = rentalContractData.rent_price + rentalContractData.charges_price,
                    deposit_price = rentalContractData.security_deposit_price
                };

                return View(viewModel);
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

        [HttpGet]
        public async Task<IActionResult> RentReceipt(int id)
        {
            var invoice = await immoDbContext.invoice.FirstOrDefaultAsync(x => x.id == id);

            if (invoice != null)
            {
                if (invoice.status == "Payée" && invoice.type != "Dépôt de garantie")
                {
                    var rentalContractData = await immoDbContext.rental_contract.FirstOrDefaultAsync(x => x.id == invoice.fk_rental_contract_id);
                    var apartment = await immoDbContext.apartment.FirstOrDefaultAsync(x => x.id == rentalContractData.fk_apartment_id);
                    var tenant = await immoDbContext.tenant.FirstOrDefaultAsync(x => x.id == rentalContractData.fk_tenant_id);
                    var lastPayment = await immoDbContext.payment.Where(p => p.fk_invoice_id == invoice.id).OrderByDescending(p => p.date_payment).FirstOrDefaultAsync();

                    if (rentalContractData != null && apartment != null && tenant != null && lastPayment != null)
                    {
                        var firstDayOfRentMonth = new DateTime(invoice.date_invoice.Year, invoice.date_invoice.Month, 1);
                        var lastDayOfRentMonth = firstDayOfRentMonth.AddMonths(1).AddDays(-1);

                        var file = Document.Create(container =>
                        {
                            container.Page(page =>
                            {
                                page.Size(PageSizes.A4);
                                page.Margin(2, Unit.Centimetre);
                                page.PageColor(Colors.White);
                                page.DefaultTextStyle(x => x.FontSize(14));

                                page.Header()
                                    .AlignCenter()
                                    .Text("Quittance de loyer du mois de " + invoice.date_invoice.ToString("MMMM yyyy")).FontColor("#002c77")
                                    .SemiBold().FontSize(22);

                                page.Content()
                                    .PaddingVertical(1, Unit.Centimetre)
                                    .Column(x =>
                                    {
                                        x.Spacing(20);

                                        x.Item().Text(text =>
                                        {
                                            text.Line($"{tenant.first_name} {tenant.last_name}");
                                            text.Line(tenant.email);
                                        });

                                        x.Item().AlignRight().Text(text =>
                                        {
                                            text.Line("Adresse de la location : ").Underline().Bold().FontColor("#135faa");
                                            text.Line($"{apartment.address} {apartment.address_complement}");
                                            text.Line($"{apartment.city} {apartment.zip_code}");
                                        });

                                        x.Item().Text($"Je déclare avoir reçu de {tenant.civility} {tenant.last_name}, la somme de {string.Format("{0:0.00}", invoice.amount)} euros, " +
                                                      $"au titre du paiement du loyer et des charges pour la période de location du {firstDayOfRentMonth:d MMMM yyyy} " +
                                                      $"au {lastDayOfRentMonth:d MMMM yyyy} et lui en donne quittance, sous réserve de tous mes droits.");

                                        x.Item().Text(text =>
                                        {
                                            text.Line("Détail du règlement : ").Underline().Bold().FontColor("#135faa");
                                            text.Line($"Loyer : {string.Format("{0:0.00}", rentalContractData.rent_price * 0.92)} euros");
                                            text.Line($"Provision pour charges : {string.Format("{0:0.00}", rentalContractData.charges_price)} euros");
                                            text.Line($"Frais d'agence : {string.Format("{0:0.00}", rentalContractData.rent_price * 0.08)} euros");
                                            text.Line($"Total : {string.Format("{0:0.00}", rentalContractData.rent_price + rentalContractData.charges_price)} euros").Bold();
                                            text.Line($"Date du paiement : le {lastPayment.date_payment.ToString("dd/MM/yyyy")}");
                                        });
                                    });

                                page.Footer()
                                    .AlignCenter()
                                    .Text(x =>
                                    {
                                        x.Span("Page ");
                                        x.CurrentPageNumber();
                                    });
                            });
                        });

                        byte[] pdfBytes = file.GeneratePdf();

                        return File(pdfBytes, "application/octet-stream", "Invoice_" + id + ".pdf");
                    }
                }

                return RedirectToAction("Detail", "rentalContracts", new { id = invoice.fk_rental_contract_id });
            }
            
            return RedirectToAction("Index", "rentalContracts");
        }
    }
}
