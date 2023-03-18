namespace Immo_App.Core.Models.RentalContract
{
    public class AddRentalContractViewModel
    {
        public float charges_price { get; set; }
        public float rent_price { get; set; }
        public float security_deposit_price { get; set; }
        public int fk_tenant_id { get; set; }
        public int fk_apartment_id { get; set; }
        public List<Apartment.Apartment> available_apartments { get; set; }
        public List<Tenant.Tenant> tenants { get; set; }
    }
}
