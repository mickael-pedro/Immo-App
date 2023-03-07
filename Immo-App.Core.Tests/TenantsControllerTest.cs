using Immo_App.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Immo_App.Core.Tests
{
    public class TenantsControllerTest
    {
        [Fact]
        public void TenantsControllerIndexTest()
        {
            // Arrange
            var controller = new TenantsController();

            // Act
            var result = controller.Index();
            var viewresult = result.Result as ViewResult;

            // Assert
            Assert.Equal("Index", viewresult.ViewName);
        }
    }
}