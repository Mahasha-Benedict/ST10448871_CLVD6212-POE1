using ABCRetailers.Models;
using ABCRetailers.Models.ViewModels;
using ABCRetailers.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ABCRetailers.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAzureStorageService _storage;

        public HomeController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _storage.GetAllCustomersAsync();
            var products = await _storage.GetAllProductsAsync();
            var orders = await _storage.GetAllOrdersAsync();

            var featuredProducts = products
                .OrderByDescending(p => p.StockAvailable)
                .Take(8)
                .ToList();

            var model = new HomeViewModel
            {
                CustomerCount = customers.Count,
                ProductCount = products.Count,
                OrderCount = orders.Count,
                FeaturedProducts = featuredProducts
            };

            return View(model);
        }
    }
}
