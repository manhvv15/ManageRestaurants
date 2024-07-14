using System;
using System.Collections.Generic;

namespace ManageRestaurantsClient.Models
{
    public partial class BookingRequest
    {
        public BookingRequest()
        {
            Reserveds = new HashSet<Reserved>();
        }

        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int? TableId { get; set; }
        public DateTime ReservationDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }

        public virtual Table? Table { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Reserved> Reserveds { get; set; }
    }
}
