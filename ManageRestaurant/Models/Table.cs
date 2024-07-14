using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Table
    {
        public Table()
        {
            BookingRequests = new HashSet<BookingRequest>();
            Orders = new HashSet<Order>();
            Reserveds = new HashSet<Reserved>();
        }

        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<BookingRequest> BookingRequests { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Reserved> Reserveds { get; set; }
    }
}
