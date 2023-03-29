using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Models.Tenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Tests
{
    public class TenantsControllerTest
    {
        [Fact]
        public void TenantsControllerIndexTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_tenant")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            context.SaveChanges();

            var controller = new TenantsController(context);

            // Act
            var result = controller.Index();
            var viewresult = result.Result as ViewResult;
            var model = (List<Tenant>)(viewresult.Model);

            // Assert
            Assert.Equal("Index", viewresult.ViewName);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void TenantsControllerAddTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_tenant")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            context.SaveChanges();

            var controller = new TenantsController(context);
            AddTenantViewModel tenantToAdd = new()
            { civility = "Monsieur", first_name = "Pierre", last_name = "Marchal", email = "p.marchal@example.fr" };

            // Act
            controller.Add(tenantToAdd);

            // Assert
            var tenantAdded = context.tenant.Find(3);
            // Check if ID and email of last added Tenant match what we except
            Assert.Equal(3, tenantAdded.id);
            Assert.Equal("p.marchal@example.fr", tenantAdded.email);
        }

        [Fact]
        public void TenantsControllerUpdateTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_tenant")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            context.SaveChanges();

            var controller = new TenantsController(context);
            Tenant tenantToUpdate = new()
            { id = 1, civility = "Monsieur", first_name = "Paul", last_name = "Dupont", email = "paul@dupont.com" };

            // Act
            controller.Edit(tenantToUpdate);

            // Assert
            var tenantUpdated = context.tenant.Find(1);
            // Check if ID, name and email of our edited Tenant match what we except
            Assert.Equal(1, tenantUpdated.id);
            Assert.Equal("Paul", tenantUpdated.first_name);
            Assert.Equal("paul@dupont.com", tenantUpdated.email);
        }
    }
}