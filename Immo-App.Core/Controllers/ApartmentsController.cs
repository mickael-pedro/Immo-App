using Immo_App.Core.Data;
using Immo_App.Core.Models;
using Immo_App.Core.Models.Apartment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Immo_App.Core.Controllers
{
    public class ApartmentsController : Controller
    {
        private readonly ImmoDbContext immoDbContext;
        public ApartmentsController(ImmoDbContext immoDbContext)
        {
            this.immoDbContext = immoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var apartments = await immoDbContext.apartment.OrderBy(a => a.id).ToListAsync();
            return View("Index", apartments);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddApartmentViewModel addApartmentRequest)
        {
            var apartment = new Apartment()
            {
                address = addApartmentRequest.address,
                address_complement = addApartmentRequest.address_complement,
                city = addApartmentRequest.city,
                zip_code = addApartmentRequest.zip_code
            };

            await immoDbContext.apartment.AddAsync(apartment);
            await immoDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
