using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public int? OrderId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal Price { get; set; }
        public string? PaymentMethod { get; set; }

        public virtual Order? Order { get; set; }
    }
}
