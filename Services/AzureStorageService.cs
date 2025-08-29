using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using ABCRetailers.Models;
using Microsoft.AspNetCore.Http;

namespace ABCRetailers.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly TableClient _productTable;
        private readonly TableClient _customerTable;
        private readonly TableClient _orderTable;
        private readonly BlobContainerClient _productImagesContainer;
        private readonly BlobContainerClient _proofsContainer;

        public AzureStorageService(string? connectionString)
        {
        
            var productTbl = "Products";
            var customerTbl = "Customers";
            var orderTbl = "Orders";
            var productImgs = "product-images";
            var proofImgs = "payment-proofs";

            // Initialize TableServiceClient and TableClients
            var tableService = new TableServiceClient(connectionString);
            _productTable = tableService.GetTableClient(productTbl);
            _customerTable = tableService.GetTableClient(customerTbl);
            _orderTable = tableService.GetTableClient(orderTbl);

            // Initialize BlobServiceClient and BlobContainerClients
            var blobService = new BlobServiceClient(connectionString);
            _productImagesContainer = blobService.GetBlobContainerClient(productImgs);
            _proofsContainer = blobService.GetBlobContainerClient(proofImgs);
        }

        // Create tables and blob containers if missing
        public async Task CreateTablesIfNotExistsAsync()
        {
            await _productTable.CreateIfNotExistsAsync();
            await _customerTable.CreateIfNotExistsAsync();
            await _orderTable.CreateIfNotExistsAsync();
            await _productImagesContainer.CreateIfNotExistsAsync();
            await _proofsContainer.CreateIfNotExistsAsync();
        }

        // ====================== PRODUCTS ======================
        public async Task AddProductAsync(Product product) =>
            await _productTable.AddEntityAsync(product);

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var list = new List<Product>();
            await foreach (var entity in _productTable.QueryAsync<Product>())
                list.Add(entity);
            return list;
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            try
            {
                var resp = await _productTable.GetEntityAsync<Product>("PRODUCT", id);
                return resp.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task UpdateProductAsync(Product product) =>
            await _productTable.UpsertEntityAsync(product, TableUpdateMode.Replace);

        public async Task DeleteProductAsync(string id) =>
            await _productTable.DeleteEntityAsync("PRODUCT", id);

        public async Task<(string url, string blobName)> UploadProductImageAsync(IFormFile file, string? preferredName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file upload");

            var blobName = preferredName ?? Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
            var blobClient = _productImagesContainer.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return (blobClient.Uri.ToString(), blobName);
        }

        public async Task DeleteProductImageAsync(string blobName) =>
            await _productImagesContainer.GetBlobClient(blobName).DeleteIfExistsAsync();

        // ====================== CUSTOMERS ======================
        public async Task AddCustomerAsync(Customer customer) =>
            await _customerTable.AddEntityAsync(customer);

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var list = new List<Customer>();
            await foreach (var entity in _customerTable.QueryAsync<Customer>())
                list.Add(entity);
            return list;
        }

        public async Task<Customer?> GetCustomerByIdAsync(string id)
        {
            try
            {
                var resp = await _customerTable.GetEntityAsync<Customer>("CUSTOMER", id);
                return resp.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task UpdateCustomerAsync(Customer customer) =>
            await _customerTable.UpsertEntityAsync(customer, TableUpdateMode.Replace);

        public async Task DeleteCustomerAsync(string id) =>
            await _customerTable.DeleteEntityAsync("CUSTOMER", id);

        // ====================== ORDERS ======================
        public async Task AddOrderAsync(Order order) =>
            await _orderTable.AddEntityAsync(order);

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var list = new List<Order>();
            await foreach (var entity in _orderTable.QueryAsync<Order>())
                list.Add(entity);
            return list;
        }

        public async Task<Order?> GetOrderByIdAsync(string id)
        {
            try
            {
                var resp = await _orderTable.GetEntityAsync<Order>("ORDER", id);
                return resp.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task UpdateOrderAsync(Order order) =>
            await _orderTable.UpsertEntityAsync(order, TableUpdateMode.Replace);

        public async Task DeleteOrderAsync(string id) =>
            await _orderTable.DeleteEntityAsync("ORDER", id);

        // ====================== PROOFS ======================
        public async Task<(string url, string blobName)> UploadProofAsync(IFormFile file, string? preferredName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file upload");

            var blobName = preferredName ?? Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
            var blobClient = _proofsContainer.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return (blobClient.Uri.ToString(), blobName);
        }

        public async Task DeleteProofAsync(string blobName) =>
            await _proofsContainer.GetBlobClient(blobName).DeleteIfExistsAsync();
    }
}
