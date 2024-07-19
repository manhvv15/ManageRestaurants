namespace ManageRestaurant.DTO
{
    public class AddBookingRequestDTO
    {
        public int? UserId { get; set; }
        public int? TableId { get; set; }
        public DateTime ReservationDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
        public bool? IsDeposited { get; set; }
        public int? depositAmount { get; set; }
    }
}
