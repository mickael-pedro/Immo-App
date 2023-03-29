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
    }
}
