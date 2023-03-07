using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Models;
using Microsoft.AspNetCore.Mvc;
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
    }
}