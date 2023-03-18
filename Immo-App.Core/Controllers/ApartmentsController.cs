using Immo_App.Core.Data;
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var apartment = await immoDbContext.apartment.FirstOrDefaultAsync(x => x.id == id);

            if (apartment != null)
            {
                return View(apartment);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Apartment model)
        {
            var apartment = await immoDbContext.apartment.FindAsync(model.id);

            if (apartment != null)
            {
                apartment.id = model.id;
                apartment.address = model.address;
                apartment.address_complement = model.address_complement;
                apartment.city = model.city;
                apartment.zip_code = model.zip_code;

                await immoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var apartment = await immoDbContext.apartment.FirstOrDefaultAsync(x => x.id == id);

            if (apartment != null)
            {
                immoDbContext.apartment.Remove(apartment);
                await immoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
