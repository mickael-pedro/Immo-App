using Immo_App.Core.Models.Apartment;
using Immo_App.Core.Models.RentalContract;
using Immo_App.Core.Models.Tenant;

namespace Immo_App.Core.Tests
{
    public class TestDataHelper
    {
        public static List<Tenant> GetFakeTenantList()
        {
            return new List<Tenant>()
            {
                new Tenant
                {
                    id = 1,
                    civility = "Monsieur",
                    first_name = "Jean",
                    last_name = "Dupont",
                    email = "jean@dupont.com"
                },
                new Tenant
                {
                    id = 2,
                    civility = "Madame",
                    first_name = "Jeanne",
                    last_name = "Pasquier",
                    email = "jeanne@exemple.org"
                }
            };
        }

        public static List<Apartment> GetFakeApartmentList()
        {
            return new List<Apartment>()
            {
                new Apartment
                {
                    id = 1,
                    address = "29 Avenue du General Michel Bizot",
                    address_complement = "App 31",
                    city = "Escatalens",
                    zip_code = 82700
                },
                new Apartment
                {
                    id = 2,
                    address = "12 Rue Boreau",
                    address_complement = "ZI Girard",
                    city = "Angers",
                    zip_code = 49100
                }
            };
        }

        public static List<RentalContract> GetFakeRentalContractList()
        {
            return new List<RentalContract>()
            {
                new RentalContract
                {
                    id = 1,
                    charges_price = 900,
                    security_deposit_price = 1200,
                    security_deposit_status = "Non payé",
                    tenant_balance = 0,
                    rental_status = "En attente paiement dépot de garantie",
                    fk_tenant_id = 1,
                    fk_apartment_id = 1
                },
                new RentalContract
                {
                    id = 2,
                    charges_price = 600,
                    security_deposit_price = 1000,
                    security_deposit_status = "Payé",
                    tenant_balance = 0,
                    rental_status = "En attente état des lieux entrée",
                    fk_tenant_id = 2,
                    fk_apartment_id = 2
                }
            };
        }
    }
}
