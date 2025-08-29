using System.Linq;
using System.Threading.Tasks;
using ABCRetailers.Models;
using ABCRetailers.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailers.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IAzureStorageService _storage;

        public CustomerController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _storage.GetAllCustomersAsync();
            return View(customers);
        }

        // GET: Details
        public async Task<IActionResult> Details(string id)
        {
            var customer = await _storage.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View(new Customer());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _storage.AddCustomerAsync(model);
            TempData["Success"] = "Customer added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(string id)
        {
            var customer = await _storage.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _storage.UpdateCustomerAsync(model);
            TempData["Success"] = "Customer updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _storage.DeleteCustomerAsync(id);
            TempData["Success"] = "Customer deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
