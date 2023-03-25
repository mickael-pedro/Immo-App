namespace Immo_App.Core.Models.RentalContract
{
	public class DetailRentalContractViewModel
	{
        public int id { get; set; }
        public float charges_price { get; set; }
        public float rent_price { get; set; }
        public float security_deposit_price { get; set; }
        public string security_deposit_status { get; set; }
        public float tenant_balance { get; set; }
        public string rental_status { get; set; }
        public bool rental_active { get; set; }
        public string tenant_name { get; set; }
        public string tenant_email { get; set; }
        public string apartment_address { get; set; }
        public List<InventoryFixture.InventoryFixture> inventory_fixtures { get; set; }
        public List<Invoice.Invoice> invoices { get; set; }
    }
}
