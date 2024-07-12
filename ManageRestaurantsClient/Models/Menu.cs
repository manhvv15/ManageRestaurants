using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Menu
    {
        public Menu()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int MenuId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
