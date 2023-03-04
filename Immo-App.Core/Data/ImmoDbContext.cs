using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Data
{
    public class ImmoDbContext : DbContext
    {
        public ImmoDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
