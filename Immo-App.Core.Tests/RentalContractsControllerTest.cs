using Immo_App.Core.Controllers;
using Immo_App.Core.Data;
using Immo_App.Core.Helpers;
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
            .UseInMemoryDatabase(databaseName: "immo_db_rental")
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
            Assert.Equal("12 Rue Boreau Appartement 13, 49100 Angers", model[1].apartment_address);
        }

        [Fact]
        public void RentalContractsControllerAddTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_rental")
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
            // Assert that the information of the rental contract added match what we except
            Assert.Equal(600, rentalContractAdded.charges_price);
            Assert.Equal(800, rentalContractAdded.rent_price);
            Assert.Equal("Non payé", rentalContractAdded.security_deposit_status);
            Assert.Equal(0, rentalContractAdded.tenant_balance);
            Assert.Equal("En attente du paiement du dépôt de garantie", rentalContractAdded.rental_status);
            Assert.True(rentalContractAdded.rental_active);
            Assert.Equal(1, rentalContractAdded.fk_tenant_id);
        }

        [Fact]
        public void RentalContractsControllerEditTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_rental")
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
            var rentalContractBeforeEdit = context.rental_contract.Find(2);
            RentalContract rentalContractToEdit = new()
            {
                id = rentalContractBeforeEdit.id,
                charges_price = 700,
                rent_price = 900,
                security_deposit_price = rentalContractBeforeEdit.security_deposit_price,
                security_deposit_status = rentalContractBeforeEdit.security_deposit_status,
                tenant_balance = rentalContractBeforeEdit.tenant_balance,
                rental_status = rentalContractBeforeEdit.rental_status,
                fk_tenant_id = rentalContractBeforeEdit.fk_tenant_id,
                fk_apartment_id = rentalContractBeforeEdit.fk_apartment_id
            };

            // Act
            controller.Edit(rentalContractToEdit);

            // Assert
            var rentalContractEdited = context.rental_contract.Find(2);
            // Assert that the updated information match what we except along with the information that shouldn't be changed 
            Assert.Equal(700, rentalContractEdited.charges_price);
            Assert.Equal(900, rentalContractEdited.rent_price);
            Assert.Equal("Payé", rentalContractEdited.security_deposit_status);
            Assert.Equal(200, rentalContractEdited.tenant_balance);
            Assert.Equal("En attente état des lieux entrée", rentalContractEdited.rental_status);
            Assert.True(rentalContractEdited.rental_active);
            Assert.Equal(2, rentalContractEdited.fk_tenant_id);
            Assert.Equal(2, rentalContractEdited.fk_apartment_id);
        }

        [Fact]
        public void RentalContractsControllerDeleteTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_rental")
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
            controller.Delete(2);
            var result = controller.Index();
            var viewresult = result.Result as ViewResult;
            var model = (List<IndexRentalContractViewModel>)(viewresult.Model);

            // Assert
            // We deleted the second contract so there should only be one left and no second contract can be found
            Assert.Equal(1, model.Count);
            Assert.Null(context.rental_contract.Find(2));
        }

        [Fact]
        public void RentalContractsControllerDetailTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_rental")
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
            var result = controller.Detail(2);
            var viewresult = result.Result as ViewResult;
            var model = (DetailRentalContractViewModel)(viewresult.Model);

            // Assert
            // We deleted the second contract so there should only be one left and no second contract can be found
            Assert.Equal(600, model.charges_price);
            Assert.Equal(800, model.rent_price);
            Assert.Equal(1000, model.security_deposit_price);
            Assert.Equal("Payé", model.security_deposit_status);
            Assert.Equal(200, model.tenant_balance);
            Assert.Equal("En attente état des lieux entrée", model.rental_status);
            Assert.True(model.rental_active);
            Assert.Equal("Madame Jeanne Pasquier", model.tenant_name);
            Assert.Equal("12 Rue Boreau Appartement 13, 49100 Angers", model.apartment_address);
        }

        [Fact]
        public void RentalContractsControllerEndContractTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_rental")
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

            var controller = new RentalContractsController(context);
            var helper = new UpdateStatusBalanceHelper(context);

            // Act
            // This shouldn't be able to end since there is no inventory fixture or paid security deposit
            controller.EndContract(1);

            // Update the rental status of our second contract to "En cours"
            helper.UpdateRentalStatus(2);
            // Then we should be able to close it
            controller.EndContract(2);

            // Assert
            var rentalContractFail = context.rental_contract.Find(1);
            var rentalContractClosed = context.rental_contract.Find(2);

            // Assert that the first contract hasn't been closed
            Assert.Equal("En attente paiement dépôt de garantie", rentalContractFail.rental_status);
            Assert.True(rentalContractFail.rental_active);

            // Assert that the second contract has been closed
            Assert.Equal("Clôturé", rentalContractClosed.rental_status);
            Assert.False(rentalContractClosed.rental_active);
        }

        [Fact]
        public void RentalContractsControllerBalanceSheetTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ImmoDbContext>()
            .UseInMemoryDatabase(databaseName: "immo_db_invoice")
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
            paymentFakeList.ForEach(p => context.payment.Add(p));
            context.SaveChanges();

            var controller = new RentalContractsController(context);

            // Act
            var result = controller.BalanceSheet(2).Result;

            // Assert
            // Assert the result type
            var fileContentResult = Assert.IsType<FileContentResult>(result);

            //Finally assert the content type and the file name
            Assert.Equal("application/octet-stream", fileContentResult.ContentType);
            Assert.Equal("BalanceSheet_2.pdf", fileContentResult.FileDownloadName);
        }
    }
}