using ManageRestaurant.DTO;
using ManageRestaurant.Models;

namespace ManageRestaurant.Interface
{
    public interface IBookingRequestRepository
    {
        Task<BookingRequest> AddBookingRequestAsync(AddBookingRequestDTO addBookingRequestDTO);
        Task<BookingRequest> GetBookingByIdAsync(int bookingId);
        Task<bool> UpdateBookingAsync(BookingRequest updatedBookingRequest);
    }
}
