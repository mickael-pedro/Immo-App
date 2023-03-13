using Immo_App.Core.Models;

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
    }
}
