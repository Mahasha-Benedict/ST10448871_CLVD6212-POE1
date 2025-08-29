using System;
using Azure;
using Azure.Data.Tables;

namespace ABCRetailers.Models
{
    public class Customer : ITableEntity
    {
        public string PartitionKey { get; set; } = "CUSTOMER";
        public string RowKey { get; set; } // Required by ITableEntity
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Domain-specific properties
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }

        // Convenience alias for RowKey
        public string CustomerRowKey
        {
            get => RowKey;
            set => RowKey = value;
        }

        public Customer()
        {
            RowKey = Guid.NewGuid().ToString();
        }
    }
}

