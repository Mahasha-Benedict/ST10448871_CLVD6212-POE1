using Azure;
using Azure.Data.Tables;

namespace ABCRetailers.Models
{
    public class Product : ITableEntity
    {
        public string PartitionKey { get; set; } = "PRODUCT";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public string ProductName { get; set; } = string.Empty;
        public double Price { get; set; }
        public int StockAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageBlobName { get; set; }

        // ITableEntity properties
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
