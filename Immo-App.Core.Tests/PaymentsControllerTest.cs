using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Models.Payment;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Tests
{
    public class PaymentsControllerTest
    {
        [Fact]
        public void PaymentsControllerAddTest()
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

            var controller = new PaymentsController(context);
            AddPaymentViewModel paymentToAdd = new()
            { date_payment = DateTime.Parse("2023-03-06"), amount = 350, origin = "Locataire", fk_invoice_id = 1, fk_rental_contract_id = 1 };

            // Act
            controller.Add(paymentToAdd);

            // Assert
            var paymentAdded = context.payment.Find(1);
            // Assert that the information of the invoice added match what we except
            Assert.Equal(DateTime.Parse("2023-03-06").ToUniversalTime(), paymentAdded.date_payment);
            Assert.Equal(350, paymentAdded.amount);
            Assert.Equal("Locataire", paymentAdded.origin);
            Assert.Equal(1, paymentAdded.fk_invoice_id);
            Assert.Equal(1, paymentAdded.fk_rental_contract_id);
        }

        [Fact]
        public void PaymentsControllerEditTest()
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
            var paymentFakeList = TestDataHelper.GetFakePaymentList();
            paymentFakeList.ForEach(i => context.payment.Add(i));
            context.SaveChanges();

            var controller = new PaymentsController(context);
            var paymentBeforeEdit = context.payment.Find(2);
            EditPaymentViewModel paymentToEdit = new()
            { id = paymentBeforeEdit.id, date_payment = DateTime.Parse("2023-03-07"), amount = 400, origin = "Caisse d’allocation familiale", fk_invoice_id = 2, fk_rental_contract_id = 2 };

            // Act
            controller.Edit(paymentToEdit);

            // Assert
            var paymentEdited = context.payment.Find(2);
            // Assert that the information of the invoice edited match what we except
            Assert.Equal(DateTime.Parse("2023-03-07").ToUniversalTime(), paymentEdited.date_payment);
            Assert.Equal(400, paymentEdited.amount);
            Assert.Equal("Caisse d’allocation familiale", paymentEdited.origin);
            Assert.Equal(paymentBeforeEdit.fk_invoice_id, paymentEdited.fk_invoice_id);
            Assert.Equal(paymentBeforeEdit.fk_rental_contract_id, paymentEdited.fk_rental_contract_id);
        }

        [Fact]
        public void PaymentsControllerDeleteTest()
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
            var paymentFakeList = TestDataHelper.GetFakePaymentList();
            paymentFakeList.ForEach(i => context.payment.Add(i));
            context.SaveChanges();

            var controller = new PaymentsController(context);

            // Act
            // Check if we can find the inventory fixture before deleting
            Assert.NotNull(context.payment.Find(1));
            controller.Delete(1);

            // Assert
            // Assert that we can't find the inventory fixture anymore
            Assert.Null(context.payment.Find(1));
        }
    }
}