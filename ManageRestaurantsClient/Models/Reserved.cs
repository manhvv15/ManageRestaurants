using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Reserved
    {
        public int ReservationId { get; set; }
        public int? UserId { get; set; }
        public int? TableId { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string? Status { get; set; }

        public virtual Table? Table { get; set; }
        public virtual User? User { get; set; }
    }
}
