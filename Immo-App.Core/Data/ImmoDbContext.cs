using Immo_App.Core.Models.Apartment;
using Immo_App.Core.Models.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Data
{
    public class ImmoDbContext : DbContext
    {
        public ImmoDbContext()
        {
        }

        public ImmoDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Tenant> tenant { get; set; }
        public virtual DbSet<Apartment> apartment { get; set; }
    }
}
