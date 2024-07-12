using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Transactions = new HashSet<Transaction>();
        }

        public int OrderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? TableId { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public string? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual Table? Table { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
