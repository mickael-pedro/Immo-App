using Immo_App.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Data
{
    public class ImmoDbContext : DbContext
    {
        public ImmoDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Tenant> tenant { get; set; } 
    }
}
