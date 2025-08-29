using ABCRetailers.Models;
using ABCRetailers.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailers.Controllers
{
    public class OrderController : Controller
    {
        private readonly IAzureStorageService _storage;

        public OrderController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _storage.GetAllOrdersAsync();
            var customers = await _storage.GetAllCustomersAsync();
            var products = await _storage.GetAllProductsAsync();

            // Populate display names
            foreach (var o in orders)
            {
                o.CustomerName = customers.FirstOrDefault(c => c.RowKey == o.CustomerId)?.Name;
                o.ProductName = products.FirstOrDefault(p => p.RowKey == o.ProductId)?.ProductName;
            }

            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var order = await _storage.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var customer = await _storage.GetCustomerByIdAsync(order.CustomerId);
            var product = await _storage.GetProductByIdAsync(order.ProductId);

            order.CustomerName = customer?.Name;
            order.ProductName = product?.ProductName;

            return View(order);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await _storage.GetAllCustomersAsync();
            ViewBag.Products = await _storage.GetAllProductsAsync();
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                order.RowKey = Guid.NewGuid().ToString();
                order.PartitionKey = "ORDER";
                order.OrderDate = DateTime.UtcNow;
                order.Status = "Pending";

                await _storage.AddOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var order = await _storage.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            ViewBag.Customers = await _storage.GetAllCustomersAsync();
            ViewBag.Products = await _storage.GetAllProductsAsync();

            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Order order)
        {
            if (id != order.RowKey) return NotFound();

            if (ModelState.IsValid)
            {
                await _storage.UpdateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: /Order/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var order = await _storage.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: /Order/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var order = await _storage.GetOrderByIdAsync(id);
            if (order != null)
            {
                await _storage.DeleteOrderAsync(id);
            }

            TempData["Success"] = "Order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

    }
}