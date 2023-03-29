using Immo_App.Core.Data;
using Immo_App.Core.Models.Invoice;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Helpers
{
    public class UpdateStatusBalanceHelper
    {
        private readonly ImmoDbContext immoDbContext;
        public UpdateStatusBalanceHelper(ImmoDbContext immoDbContext)
        {
            this.immoDbContext = immoDbContext;
        }

        public async Task UpdateBalance(int rentalId)
        {
            var rentalContract = immoDbContext.rental_contract.Find(rentalId);

            if (rentalContract != null)
            {
                var sumInvoices = immoDbContext.invoice.Where(i => i.fk_rental_contract_id == rentalId).Sum(i => i.amount);
                var sumPayments = immoDbContext.payment.Where(p => p.fk_rental_contract_id == rentalId).Sum(p => p.amount);

                rentalContract.tenant_balance = sumPayments - sumInvoices;
                await immoDbContext.SaveChangesAsync();
                return;
            }

            return;
        }

        public async Task UpdateRentalStatus(int rentalId)
        {
            var rentalContract = immoDbContext.rental_contract.Find(rentalId);

            if (rentalContract != null)
            {
                var entryInventoryFixture = await immoDbContext.inventory_fixture.FirstOrDefaultAsync(i => i.fk_rental_contract_id == rentalId && i.type == "Entrée");
                var paidSecurityDeposit = await immoDbContext.invoice.FirstOrDefaultAsync(i => i.fk_rental_contract_id == rentalId && i.type == "Dépôt de garantie" && i.status == "Payée");

                // Check if no Entry Inventory Fixture has been found
                if (entryInventoryFixture == null)
                {
                    rentalContract.rental_status = "En attente état des lieux d’entrée";

                    // Update the security deposit status
                    rentalContract.security_deposit_status = paidSecurityDeposit == null ? "Non payé" : "Payé";
                }
                // Check if no paid Security Deposit Invoice has been found
                else if (paidSecurityDeposit == null)
                {
                    rentalContract.rental_status = "En attente paiement dépôt de garantie";
                    rentalContract.security_deposit_status = "Non payé";
                }
                // If both has been found, then this means the rental contract is good to go
                else
                {
                    rentalContract.rental_status = "En cours";
                    rentalContract.security_deposit_status = "Payé";
                }
             
                await immoDbContext.SaveChangesAsync();
                return;
            }

            return;
        }
    }
}
