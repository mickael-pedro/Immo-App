using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Models.Apartment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Tests
{
    public class ApartmentsControllerTest
    {
        [Fact]
        public void ApartmentsControllerIndexTest()
        {
			var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(t => context.apartment.Add(t));
            context.SaveChanges();

            var controller = new ApartmentsController(context);

            // Act
            var result = controller.Index();
            var viewresult = result.Result as ViewResult;
            var model = (List<Apartment>)(viewresult.Model);

            // Assert
            Assert.Equal("Index", viewresult.ViewName);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void ApartmentsControllerAddTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db")
            .Options;
            var context = new ImmoDbContext(options);
			context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(t => context.apartment.Add(t));
            context.SaveChanges();

            var controller = new ApartmentsController(context);
            AddApartmentViewModel apartmentToAdd = new()
            { address = "5 Rue Massena", address_complement = "Aucun", city = "Nice", zip_code = 06000 };

            // Act
            controller.Add(apartmentToAdd);

            // Assert
            var apartmentAdded = context.apartment.Find(3);
            // Check if ID and address of last added Apartment match what we except
            Assert.Equal(3, apartmentAdded.id);
            Assert.Equal("5 Rue Massena", apartmentAdded.address);
        }

        [Fact]
        public void ApartmentsControllerUpdateTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(t => context.apartment.Add(t));
            context.SaveChanges();

            var controller = new ApartmentsController(context);
            Apartment apartmentToUpdate = new()
            { id = 2, address = "24 Rue Boreau", address_complement = "ZI Jean-Jacques", city = "Angers", zip_code = 49100 };

            // Act
            controller.Edit(apartmentToUpdate);

            // Assert
            var apartmentUpdated = context.apartment.Find(2);
            // Check if ID, address and address addition of our edited Apartment match what we except
            Assert.Equal(2, apartmentUpdated.id);
            Assert.Equal("24 Rue Boreau", apartmentUpdated.address);
            Assert.Equal("ZI Jean-Jacques", apartmentUpdated.address_complement);
        }

        [Fact]
        public void ApartmentsControllerDeleteTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db")
            .Options;
            var context = new ImmoDbContext(options);
            context.Database.EnsureDeleted();

            var apartmentFakeList = TestDataHelper.GetFakeApartmentList();
            apartmentFakeList.ForEach(t => context.apartment.Add(t));
            context.SaveChanges();

            var controller = new ApartmentsController(context);

            // Act
            controller.Delete(1);
            var result = controller.Index();
            var viewresult = result.Result as ViewResult;
            var model = (List<Apartment>)(viewresult.Model);

            // Assert
            Assert.Equal(1, model.Count);
            Assert.Null(context.apartment.Find(1));
        }
    }
}