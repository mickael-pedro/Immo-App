using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Models.RentalContract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Tests
{
    public class RentalContractsControllerTest
    {
        [Fact]
        public void RentalContractsControllerIndexTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(a => context.apartment.Add(a));
            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            var rentalContractFakeList = TestDataHelper.GetFakeRentalContractList();
            rentalContractFakeList.ForEach(r => context.rental_contract.Add(r));
            context.SaveChanges();

            var controller = new RentalContractsController(context);

            // Act
            var result = controller.Index();
            var viewresult = result.Result as ViewResult;
            var model = (List<IndexRentalContractViewModel>)(viewresult.Model);

            // Assert
            // Assert that tenant's name and apartment's address tied to the rental contract match what we except
            Assert.Equal("Index", viewresult.ViewName);
            Assert.Equal("Jean Dupont", model[0].tenant_name);
            Assert.Equal("29 Avenue du General Michel Bizot, 82700 Escatalens", model[0].apartment_address);
            Assert.Equal("Jeanne Pasquier", model[1].tenant_name);
            Assert.Equal("12 Rue Boreau, 49100 Angers", model[1].apartment_address);
        }

        [Fact]
        public void RentalContractsControllerAddTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(a => context.apartment.Add(a));
            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            context.SaveChanges();

            var controller = new RentalContractsController(context);
            AddRentalContractViewModel rentalContractToAdd = new()
            { charges_price = 600, rent_price = 800, security_deposit_price = 900, fk_tenant_id = 1, fk_apartment_id = 2 };

            // Act
            controller.Add(rentalContractToAdd);

            // Assert
            var rentalContractAdded = context.rental_contract.Find(1);
            // Assert that tenant's name and apartment's address tied to the rental contract match what we except
            Assert.Equal(600, rentalContractAdded.charges_price);
            Assert.Equal(800, rentalContractAdded.rent_price);
            Assert.Equal("Non payé", rentalContractAdded.security_deposit_status);
            Assert.Equal(0, rentalContractAdded.tenant_balance);
            Assert.Equal("En attente du paiement du dépôt de garantie", rentalContractAdded.rental_status);
            Assert.Equal(true, rentalContractAdded.rental_active);
            Assert.Equal(1, rentalContractAdded.fk_tenant_id);
        }
    }
}