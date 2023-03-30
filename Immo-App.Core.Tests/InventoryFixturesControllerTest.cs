using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Models.Apartment;
using Immo_App.Core.Models.InventoryFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Tests
{
    public class InventoryFixturesControllerTest
    {
        [Fact]
        public void InventoryFixturesControllerAddTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_inventory_inventory")
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

            var controller = new InventoryFixturesController(context);
            AddInventoryFixtureViewModel inventoryFixtureToAdd = new()
            { date_inv = DateTime.Parse("2023-02-04"), type = "Entrée", notes = "Test notes", fk_rental_contract_id = 1 };

            // Act
            controller.Add(inventoryFixtureToAdd);

            // Assert
            var inventoryFixtureAdded = context.inventory_fixture.Find(1);
            // Assert that the information of the inventory fixture added match what we except
            Assert.Equal(DateTime.Parse("2023-02-04").ToUniversalTime(), inventoryFixtureAdded.date_inv);
            Assert.Equal("Entrée", inventoryFixtureAdded.type);
            Assert.Equal("Test notes", inventoryFixtureAdded.notes);
            Assert.Equal(1, inventoryFixtureAdded.fk_rental_contract_id);
        }

        [Fact]
        public void InventoryFixturesControllerEditTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_inventory")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(a => context.apartment.Add(a));
            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            var rentalContractFakeList = TestDataHelper.GetFakeRentalContractList();
            rentalContractFakeList.ForEach(r => context.rental_contract.Add(r));
            var inventoryFixtureFakeList = TestDataHelper.GetFakeInventoryFixtureList();
            inventoryFixtureFakeList.ForEach(i => context.inventory_fixture.Add(i));
            context.SaveChanges();

            var controller = new InventoryFixturesController(context);
            var inventoryFixtureBeforeEdit = context.inventory_fixture.Find(1);
            InventoryFixture inventoryFixtureToEdit = new()
            { id = inventoryFixtureBeforeEdit.id, date_inv = DateTime.Parse("2023-02-04"), type = "Entrée", notes = "Test Edit", fk_rental_contract_id = 1 };

            // Act
            controller.Edit(inventoryFixtureToEdit);

            // Assert
            var inventoryFixtureEdited = context.inventory_fixture.Find(1);
            // Assert that the information of the inventory fixture edited match what we except
            Assert.Equal(DateTime.Parse("2023-02-04").ToUniversalTime(), inventoryFixtureEdited.date_inv);
            Assert.Equal(inventoryFixtureBeforeEdit.type, inventoryFixtureEdited.type);
            Assert.Equal("Test Edit", inventoryFixtureEdited.notes);
            Assert.Equal(inventoryFixtureBeforeEdit.fk_rental_contract_id, inventoryFixtureEdited.fk_rental_contract_id);
        }

        [Fact]
        public void InventoryFixturesControllerDeleteTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_inventory")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(a => context.apartment.Add(a));
            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            var rentalContractFakeList = TestDataHelper.GetFakeRentalContractList();
            rentalContractFakeList.ForEach(r => context.rental_contract.Add(r));
            var inventoryFixtureFakeList = TestDataHelper.GetFakeInventoryFixtureList();
            inventoryFixtureFakeList.ForEach(i => context.inventory_fixture.Add(i));
            context.SaveChanges();

            var controller = new InventoryFixturesController(context);

            // Act
            // Check if we can find the inventory fixture before deleting
            Assert.NotNull(context.inventory_fixture.Find(1));
            controller.Delete(1);

            // Assert
            // Assert that we can't find the inventory fixture anymore
            Assert.Null(context.inventory_fixture.Find(1));
        }

        [Fact]
        public void InventoryFixturesControllerNotesTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_inventory")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(a => context.apartment.Add(a));
            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            var rentalContractFakeList = TestDataHelper.GetFakeRentalContractList();
            rentalContractFakeList.ForEach(r => context.rental_contract.Add(r));
            var inventoryFixtureFakeList = TestDataHelper.GetFakeInventoryFixtureList();
            inventoryFixtureFakeList.ForEach(i => context.inventory_fixture.Add(i));
            context.SaveChanges();

            var controller = new InventoryFixturesController(context);

            // Act
            var result = controller.Notes(1);
            var viewresult = result.Result as ViewResult;
            var model = (NotesInventoryFixtureViewModel)(viewresult.Model);


            // Assert
            // Assert that we can't find the inventory fixture anymore
            Assert.Equal(model.notes, "Test Fake List");
            Assert.Equal(model.apartment_address, "12 Rue Boreau Appartement 13, 49100 Angers");
        }
    }
}