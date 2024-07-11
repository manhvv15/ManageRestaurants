using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Restaurant
    {
        public Restaurant()
        {
            Menus = new HashSet<Menu>();
            Orders = new HashSet<Order>();
            Promotions = new HashSet<Promotion>();
            StaffSchedules = new HashSet<StaffSchedule>();
            Tables = new HashSet<Table>();
        }

        public int RestaurantId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public decimal? Rating { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual ICollection<StaffSchedule> StaffSchedules { get; set; }
        public virtual ICollection<Table> Tables { get; set; }
    }
}
