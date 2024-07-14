namespace ManageRestaurantsClient.DTO
{
    public class BookingRequestDTO
    {
        public class Booking
        {
            public int BookingId { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public int TableId { get; set; }
            public int TableNumber { get; set; }
            public DateTime ReservationDate { get; set; }
            public int NumberOfGuests { get; set; }
            public string Status { get; set; }
            public string Note { get; set; }
        }

        public class BookingList
        {
            public List<Booking> Bookings { get; set; }
        }
    }
}
