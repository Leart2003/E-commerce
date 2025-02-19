using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Punim_Diplome.Models;
using System.Diagnostics;

namespace Punim_Diplome.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<IActionResult> IndexAsync(String searchString, String brand)
        {
            var produktetquery = context.Produktet.OrderByDescending(p => p.Id).ToList();

            if (!string.IsNullOrEmpty(brand))
            {
                produktetquery = produktetquery.Where(p => p.Brand == brand).ToList();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                produktetquery = produktetquery.Where(p => p.Name!.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            var availableBrands = await context.Produktet
            .Select(p => p.Brand)
            .Distinct()
            .ToListAsync();


            var viewModel = new ProductVM()
            {
                Products = produktetquery.ToList(),
                SearchString = searchString,
                SelectedBrand = brand,
                AvailableBrands = availableBrands
            };


            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
