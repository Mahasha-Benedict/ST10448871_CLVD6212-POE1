using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ABCRetailers.Models;
using ABCRetailers.Services;

namespace ABCRetailers.Controllers
{
    public class UploadController : Controller
    {
        private readonly IAzureStorageService _storage;

        public UploadController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        public async Task<IActionResult> Index(string orderId, string customerName)
        {
            ViewBag.OrderId = orderId;
            ViewBag.CustomerName = customerName;

            Order order = null;
            Customer customer = null;
            Product product = null;
            double total = 0;

            // Search by Order ID first
            if (!string.IsNullOrEmpty(orderId))
            {
                order = await _storage.GetOrderByIdAsync(orderId);
            }

            // If Order ID not found and customerName is provided, try to find order by customer
            if (order == null && !string.IsNullOrEmpty(customerName))
            {
                var allCustomers = await _storage.GetAllCustomersAsync();
                customer = allCustomers.FirstOrDefault(c => c.Name.Equals(customerName, StringComparison.OrdinalIgnoreCase));

                if (customer != null)
                {
                    var allOrders = await _storage.GetAllOrdersAsync();
                    order = allOrders.FirstOrDefault(o => o.CustomerId == customer.RowKey);
                }
            }

            if (order == null)
            {
                ViewBag.ErrorMessage = "Order not found.";
                return View();
            }

            // If customer not loaded yet, load from order
            customer ??= await _storage.GetCustomerByIdAsync(order.CustomerId);

            // Load product
            product = await _storage.GetProductByIdAsync(order.ProductId);
            total = (double)(order.Quantity * (product?.Price ?? 0));

            ViewBag.Order = order;
            ViewBag.Customer = customer;
            ViewBag.Product = product;
            ViewBag.Total = total;

            return View();
        }

    }
}