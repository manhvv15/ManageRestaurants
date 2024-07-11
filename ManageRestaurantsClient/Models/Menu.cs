using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Menu
    {
        public Menu()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Orders = new HashSet<Order>();
        }

        public int MenuId { get; set; }
        public int? RestaurantId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
