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
                    last_name = "Dupont"
                },
                new Tenant
                {
                    id = 2,
                    civility = "Madame",
                    first_name = "Jeanne",
                    last_name = "Pasquier"
                }
            };
        }
    }
}
