using Immo_App.Core.Models.Apartment;
using Immo_App.Core.Models.InventoryFixture;
using Immo_App.Core.Models.Invoice;
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
                    city = "Escatalens",
                    zip_code = 82700
                },
                new Apartment
                {
                    id = 2,
                    address = "12 Rue Boreau",
                    address_complement = "Appartement 13",
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
                    rent_price = 1000,
                    security_deposit_price = 1200,
                    security_deposit_status = "Non payé",
                    tenant_balance = 0,
                    rental_status = "En attente paiement dépot de garantie",
                    rental_active = true,
                    fk_tenant_id = 1,
                    fk_apartment_id = 1
                },
                new RentalContract
                {
                    id = 2,
                    charges_price = 600,
                    rent_price = 800,
                    security_deposit_price = 1000,
                    security_deposit_status = "Payé",
                    tenant_balance = 200,
                    rental_status = "En attente état des lieux entrée",
                    rental_active = true,
                    fk_tenant_id = 2,
                    fk_apartment_id = 2
                }
            };
        }

        public static List<InventoryFixture> GetFakeInventoryFixtureList()
        {
            return new List<InventoryFixture>()
            {
                new InventoryFixture
                {
                    id = 1,
                    date_inv = DateTime.Parse("2023-01-06"),
                    type = "Entrée",
                    notes = "Test Fake List",
                    fk_rental_contract_id = 2
                }
            };
        }

        public static List<Invoice> GetFakeInvoiceList()
        {
            return new List<Invoice>()
            {
                new Invoice
                {
                    id = 1,
                    date_invoice = DateTime.Parse("2023-02-05"),
                    amount = 350,
                    type = "Loyer",
                    status = "Non payé",
                    fk_rental_contract_id = 2
                },
                new Invoice
                {
                    id = 2,
                    date_invoice = DateTime.Parse("2023-03-05"),
                    amount = 350,
                    type = "Loyer",
                    status = "Non payé",
                    fk_rental_contract_id = 2
                }
            };
        }
    }
}
