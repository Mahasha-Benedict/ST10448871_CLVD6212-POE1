using System;
using Azure;
using Azure.Data.Tables;

namespace ABCRetailers.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "ORDER";
        public string RowKey { get; set; } = Guid.NewGuid().ToString(); // OrderId
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Foreign Keys
        public string CustomerId { get; set; }
        public string ProductId { get; set; }

        // Order Info
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // ✅ New property
        public string Status { get; set; } = "Pending";

        // Extra (for display only, not stored in Table)
        public string? CustomerName { get; set; }
        public string? ProductName { get; set; }

        // Proof of payment reference
        public string? ProofUrl { get; set; }      // URL to blob
        public string? ProofBlobName { get; set; } // Blob filename for deletion

    }
}

