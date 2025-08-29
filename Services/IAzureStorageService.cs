using System.Collections.Generic;
using System.Threading.Tasks;
using ABCRetailers.Models;
using Microsoft.AspNetCore.Http;

namespace ABCRetailers.Services
{
    public interface IAzureStorageService
    {
        // ✅ Create tables and blob containers
        Task CreateTablesIfNotExistsAsync();

        // ====================== PRODUCTS ======================
        Task AddProductAsync(Product product);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(string id);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(string id);
        Task<(string url, string blobName)> UploadProductImageAsync(IFormFile file, string? preferredName = null);
        Task DeleteProductImageAsync(string blobName);

        // ====================== CUSTOMERS ======================
        Task AddCustomerAsync(Customer customer);
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(string id);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(string id);

        // ====================== ORDERS ======================
        Task AddOrderAsync(Order order);
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(string id);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(string id);

        // ====================== PROOFS ======================
        Task<(string url, string blobName)> UploadProofAsync(IFormFile file, string? preferredName = null);
        Task DeleteProofAsync(string blobName);
    }
}


