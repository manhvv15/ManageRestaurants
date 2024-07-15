using ManageRestaurant.DTO;
using ManageRestaurant.Interface;
using ManageRestaurant.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageRestaurant.Repository
{
    public class BookingRequestRepository : IBookingRequestRepository
    {
        private readonly ManageRestaurantContext _context;
        public BookingRequestRepository(IConfiguration configuration, ManageRestaurantContext context)
        {
            _context = context;
        }

        public async Task<BookingRequest> AddBookingRequestAsync(AddBookingRequestDTO addBookingRequestDTO)
        {
            try
            {
                var bookingRequest = new BookingRequest
                {
                    TableId = addBookingRequestDTO.TableId,
                    UserId = addBookingRequestDTO.UserId,
                    Note = addBookingRequestDTO.Note,
                    NumberOfGuests = addBookingRequestDTO.NumberOfGuests,
                    ReservationDate = addBookingRequestDTO.ReservationDate,
                    Status = addBookingRequestDTO.Status,
                };
                await _context.BookingRequests.AddAsync(bookingRequest);
                await _context.SaveChangesAsync();

                return bookingRequest;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add booking request", ex);
            }
        }

        public async Task<BookingRequest> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var bookingRequest = await _context.BookingRequests.FirstOrDefaultAsync(x => x.BookingId == bookingId);

                return bookingRequest;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add booking request", ex);
            }
        }

        public async Task<bool> UpdateBookingAsync(BookingRequest updatedBookingRequest)
        {
            try
            {
                var bookingRequest = await _context.BookingRequests.FindAsync(updatedBookingRequest.BookingId);

                if (bookingRequest == null)
                {
                    return false; 
                }

                bookingRequest.TableId = updatedBookingRequest.TableId;
                bookingRequest.UserId = updatedBookingRequest.UserId;
                bookingRequest.Note = updatedBookingRequest.Note;
                bookingRequest.NumberOfGuests = updatedBookingRequest.NumberOfGuests;
                bookingRequest.ReservationDate = updatedBookingRequest.ReservationDate;
                bookingRequest.Status = updatedBookingRequest.Status;

                _context.Entry(bookingRequest).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update booking request", ex);
            }
        }
    }
}
