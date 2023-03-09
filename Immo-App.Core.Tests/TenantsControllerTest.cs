using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Immo_App.Core.Tests
{
    public class TenantsControllerTest
    {
        [Fact]
        public void TenantsControllerIndexTest()
        {
            // Arrange
            var dbContextMock = new Mock<ImmoDbContext>();
            dbContextMock.Setup(x => x.tenant).ReturnsDbSet(TestDataHelper.GetFakeTenantList());
            var controller = new TenantsController(dbContextMock.Object);

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
            .UseInMemoryDatabase(databaseName: "immo_db")
            .Options;
            var context = new ImmoDbContext(options);

            var tenantFakeList = TestDataHelper.GetFakeTenantList();
            tenantFakeList.ForEach(t => context.tenant.Add(t));
            context.SaveChanges();

            var controller = new TenantsController(context);
            AddTenantViewModel tenantToAdd = new AddTenantViewModel()
            { civility = "Monsieur", first_name = "Pierre", last_name = "Marchal", email = "p.marchal@example.fr" };

            // Act

            // Assert
        }
    }
}