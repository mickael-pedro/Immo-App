namespace Immo_App.Core.Models.RentalContract
{
	public class RentalContract
	{
        public int id { get; set; }
        public float charges_price { get; set; }
        public float rent_price { get; set; }
        public float security_deposit_price { get; set; }
        public string security_deposit_status { get; set; }
        public float tenant_balance { get; set; }
        public string rental_status { get; set; }
        public bool rental_active { get; set; }
        public int fk_tenant_id { get; set; }
        public int fk_apartment_id { get; set; }
    }
}
