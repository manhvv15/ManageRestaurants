using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Payments = new HashSet<Payment>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public int? RestaurantId { get; set; }
        public int? Quantity { get; set; }
        public int? MenuId { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public virtual Menu? Menu { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
