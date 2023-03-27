namespace Immo_App.Core.Models.Payment
{
    public class AddPaymentViewModel
    {
        public DateTime date_payment { get; set; }
        public float amount { get; set; }
        public string origin { get; set; }
        public int fk_invoice_id { get; set; }
        public int fk_rental_contract_id { get; set; }
        public List<Invoice.Invoice> invoices { get; set; }
    }
}
