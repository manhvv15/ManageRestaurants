using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class User
    {
        public User()
        {
            BookingRequests = new HashSet<BookingRequest>();
            Orders = new HashSet<Order>();
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
        public DateTime? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public virtual ICollection<BookingRequest> BookingRequests { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
