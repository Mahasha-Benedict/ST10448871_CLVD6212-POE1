using System;
using System.Threading.Tasks;
using ABCRetailers.Models;
using ABCRetailers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailers.Controllers
{
    public class ProductController : Controller
    {
        private readonly IAzureStorageService _storage;

        public ProductController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _storage.GetAllProductsAsync();
            return View(products);
        }

        public IActionResult Create() => View(new Product());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                if (imageFile is { Length: > 0 })
                {
                    var (url, blobName) = await _storage.UploadProductImageAsync(imageFile);
                    model.ImageUrl = url;
                    model.ImageBlobName = blobName;
                }

                // ✅ Ensure price is decimal
                model.Price = Convert.ToDouble(model.Price);

                await _storage.AddProductAsync(model);
                TempData["Success"] = "Product added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var product = await _storage.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = await _storage.GetProductByIdAsync(model.RowKey);
            if (existing == null) return NotFound();

            existing.ProductName = model.ProductName;
            existing.Price = Convert.ToDouble(model.Price);
            existing.StockAvailable = model.StockAvailable;

            try
            {
                if (imageFile is { Length: > 0 })
                {
                    var (url, blobName) = await _storage.UploadProductImageAsync(imageFile);

                    if (!string.IsNullOrWhiteSpace(existing.ImageBlobName))
                        await _storage.DeleteProductImageAsync(existing.ImageBlobName);

                    existing.ImageUrl = url;
                    existing.ImageBlobName = blobName;
                }

                await _storage.UpdateProductAsync(existing);
                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Update failed: {ex.Message}");
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var product = await _storage.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var existing = await _storage.GetProductByIdAsync(id);
            if (existing != null)
            {
                await _storage.DeleteProductAsync(existing.RowKey);
                if (!string.IsNullOrWhiteSpace(existing.ImageBlobName))
                    await _storage.DeleteProductImageAsync(existing.ImageBlobName);
            }

            TempData["Success"] = "Product deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}

