namespace Immo_App.Core.Models.InventoryFixture
{
	public class InventoryFixture
    {
		public int id { get; set; }
		public DateTime date_inv { get; set; }
		public string type { get; set; }
		public string notes { get; set; }
		public int fk_rental_contract_id { get; set; }
	}
}
