namespace Immo_App.Core.Models.Tenant
{
    public class UpdateTenantViewModel
    {
        public int id { get; set; }
        public string civility { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string email { get; set; }
    }
}
