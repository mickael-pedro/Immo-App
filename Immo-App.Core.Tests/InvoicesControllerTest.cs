using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Models.Invoice;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Tests
{
    public class InvoicesControllerTest
    {
        [Fact]
        public void InvoicesControllerAddTest()
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

            var controller = new InvoicesController(context);
            AddInvoiceViewModel invoiceToAdd = new()
            { date_invoice = DateTime.Parse("2023-03-05"), amount = 300, type = "Dépôt de garantie", fk_rental_contract_id = 1 };

            // Act
            controller.Add(invoiceToAdd);

            // Assert
            var invoiceAdded = context.invoice.Find(1);
            // Assert that the information of the invoice added match what we except
            Assert.Equal(DateTime.Parse("2023-03-05").ToUniversalTime(), invoiceAdded.date_invoice);
            Assert.Equal(300, invoiceAdded.amount);
            Assert.Equal("Dépôt de garantie", invoiceAdded.type);
            Assert.Equal("Non payé", invoiceAdded.status);
            Assert.Equal(1, invoiceAdded.fk_rental_contract_id);
        }

        [Fact]
        public void InvoicesControllerEditTest()
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
            var invoiceFakeList = TestDataHelper.GetFakeInvoiceList();
            invoiceFakeList.ForEach(i => context.invoice.Add(i));
            context.SaveChanges();

            var controller = new InvoicesController(context);
            var invoiceBeforeEdit = context.invoice.Find(1);
            Invoice invoiceToEdit = new()
            { id = invoiceBeforeEdit.id, date_invoice = DateTime.Parse("2023-02-04"), amount = 400, type = "Loyer", status = "Non payé", fk_rental_contract_id = 1 };

            // Act
            controller.Edit(invoiceToEdit);

            // Assert
            var invoiceEdited = context.invoice.Find(1);
            // Assert that the information of the invoice edited match what we except
            Assert.Equal(DateTime.Parse("2023-02-04").ToUniversalTime(), invoiceEdited.date_invoice);
            Assert.Equal(400, invoiceEdited.amount);
            Assert.Equal(invoiceBeforeEdit.type, invoiceEdited.type);
            Assert.Equal(invoiceEdited.status, invoiceEdited.status);
            Assert.Equal(invoiceBeforeEdit.fk_rental_contract_id, invoiceEdited.fk_rental_contract_id);
        }

        [Fact]
        public void InvoicesControllerDeleteTest()
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
            var invoiceFakeList = TestDataHelper.GetFakeInvoiceList();
            invoiceFakeList.ForEach(i => context.invoice.Add(i));
            context.SaveChanges();

            var controller = new InvoicesController(context);

            // Act
            // Check if we can find the inventory fixture before deleting
            Assert.NotNull(context.invoice.Find(1));
            controller.Delete(1);

            // Assert
            // Assert that we can't find the inventory fixture anymore
            Assert.Null(context.invoice.Find(1));
        }
    }
}