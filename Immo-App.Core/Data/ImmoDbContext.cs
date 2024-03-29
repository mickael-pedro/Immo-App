﻿using Immo_App.Core.Models.Apartment;
using Immo_App.Core.Models.InventoryFixture;
using Immo_App.Core.Models.Invoice;
using Immo_App.Core.Models.Payment;
using Immo_App.Core.Models.RentalContract;
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
        public virtual DbSet<RentalContract> rental_contract { get; set; }
        public virtual DbSet<InventoryFixture> inventory_fixture { get; set; }
        public virtual DbSet<Invoice> invoice { get; set; }
        public virtual DbSet<Payment> payment { get; set; }
    }
}
