using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Reserved
    {
        public int ReservedId { get; set; }
        public DateTime ReservedDate { get; set; }
        public int? TableId { get; set; }
        public int? BookingId { get; set; }

        public virtual BookingRequest? Booking { get; set; }
        public virtual Table? Table { get; set; }
    }
}
