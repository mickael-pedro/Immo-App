﻿namespace Immo_App.Core.Models.Invoice
{
	public class AddInvoiceViewModel
    {
		public DateTime date_invoice { get; set; }
		public float amount { get; set; }
		public string type { get; set; }
		public int fk_rental_contract_id { get; set; }
		public float rent_charges_sum { get; set; }
        public float deposit_price { get; set; }
    }
}
