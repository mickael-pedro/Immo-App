using Immo_App.Core.Data;
using Immo_App.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Tests
{
    public class UpdateStatusBalanceHelperTest
    {
        [Fact]
        public void UpdateBalanceTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_status_balance")
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

            var helper = new UpdateStatusBalanceHelper(context);

            // Act
            // Balance of rental contract is 0 before the update
            helper.UpdateBalance(1);

            // Assert
            var rentalContract = context.rental_contract.Find(1);
            // Assert that the balance has been correctly updated (Invoice is 350 and payment is 300)
            Assert.Equal(-50, rentalContract.tenant_balance);
        }

        [Fact]
        public void UpdateRentalStatusTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_status_balance")
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
            var invoiceFakeList = TestDataHelper.GetFakeInvoiceList();
            invoiceFakeList.ForEach(i => context.invoice.Add(i));
            var paymentFakeList = TestDataHelper.GetFakePaymentList();
            paymentFakeList.ForEach(i => context.payment.Add(i));
            context.SaveChanges();

            var helper = new UpdateStatusBalanceHelper(context);

            // Act
            // Status of rental contract is "En attente état des lieux entrée" before the update
            helper.UpdateRentalStatus(2);

            // Assert
            var rentalContract = context.rental_contract.Find(2);
            // Assert that the Status has been correctly updated
            Assert.Equal("En cours", rentalContract.rental_status);
        }

        [Fact]
        public void UpdateInvoiceStatusTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_status_balance")
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
            var invoiceFakeList = TestDataHelper.GetFakeInvoiceList();
            invoiceFakeList.ForEach(i => context.invoice.Add(i));
            var paymentFakeList = TestDataHelper.GetFakePaymentList();
            paymentFakeList.ForEach(i => context.payment.Add(i));
            context.SaveChanges();

            var helper = new UpdateStatusBalanceHelper(context);

            // Act
            // Balance of rental contract is "Non payée"
            helper.UpdateInvoiceStatus(3);

            // Assert
            var invoice = context.invoice.Find(3);
            // Assert that the invoice status has been correctly updated
            Assert.Equal("Payée", invoice.status);
        }

        [Fact]
        public void AddPaidSecurityDepositTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_status_balance")
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

            var helper = new UpdateStatusBalanceHelper(context);

            // Act
            // Balance of rental contract is "Non payée"
            helper.AddPaidSecurityDeposit(1, 1200, "Locataire");

            // Assert
            var rentalContract = context.rental_contract.Find(1);
            var invoice = context.invoice.Find(1);
            var payment = context.payment.Find(1);

            // Assert that the invoice and payment has been correctly added, and the rental contract has been updated
            Assert.Equal(1200, invoice.amount);
            Assert.Equal("Payée", invoice.status);
            Assert.Equal("Dépôt de garantie", invoice.type);

            Assert.Equal(1200, payment.amount);
            Assert.Equal("Locataire", payment.origin);

            Assert.Equal("Payé", rentalContract.security_deposit_status);
        }
    }
}