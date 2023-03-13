using Immo_App.Core.Data;
using Immo_App.Core.Models;
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
    }
}
