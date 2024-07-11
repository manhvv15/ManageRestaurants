using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            Reserveds = new HashSet<Reserved>();
            StaffSchedules = new HashSet<StaffSchedule>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public double? Balance { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Reserved> Reserveds { get; set; }
        public virtual ICollection<StaffSchedule> StaffSchedules { get; set; }
    }
}
