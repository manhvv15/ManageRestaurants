using System;
using System.Collections.Generic;

namespace ManageRestaurantsClient.Models
{
    public partial class Transaction
    {
        public int TransactionId { get; set; }
        public int? OrderId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal Price { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public virtual Order? Order { get; set; }
    }
}
