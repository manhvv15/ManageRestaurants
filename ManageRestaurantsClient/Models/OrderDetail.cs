using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? MenuId { get; set; }
        public int? Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual Menu? Menu { get; set; }
        public virtual Order? Order { get; set; }
    }
}
