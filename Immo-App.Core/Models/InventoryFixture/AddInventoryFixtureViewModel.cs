namespace Immo_App.Core.Models.InventoryFixture
{
	public class AddInventoryFixtureViewModel
    {
		public DateTime date_inv { get; set; }
		public string type { get; set; }
		public string notes { get; set; }
		public int fk_rental_contract_id { get; set; }
	}
}
