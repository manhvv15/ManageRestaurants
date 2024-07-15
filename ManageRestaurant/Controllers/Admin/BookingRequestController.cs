using ManageRestaurant.DTO;
using ManageRestaurant.Models;
using ManageRestaurant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageRestaurant.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingRequestController : ControllerBase
    {
        private readonly ManageRestaurantContext context ;
        private readonly TableController tableController;
        DateTime reservationDate;

        Email email = new Email();
        public BookingRequestController(ManageRestaurantContext context)
        {
            this.context = context;
            this.tableController = new TableController(context);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        //public async Task<ActionResult> getAllBooking(int pageNumber = 1, int pageSize = 10)
        //{
        //    var bookings = await context.BookingRequests
        //                         .Include(t => t.Table)
        //                         .Include(u => u.User)
        //                         .Skip((pageNumber - 1) * pageSize)
        //                         .Take(pageSize)
        //                         .ToListAsync();

        //    var bookingList = bookings.Select(b => new BookingRequestDTO
        //    {
        //        BookingId = b.BookingId,
        //        UserId = (int)b.UserId,
        //        UserName = b.User.UserName,
        //        Email = b.User.Email,
        //        TableId = (int)b.TableId,
        //        TableNumber = b.Table.TableNumber,
        //        ReservationDate = b.ReservationDate,
        //        NumberOfGuests = b.NumberOfGuests,
        //        Status = b.Status,
        //        Note = b.Note
        //    });

        //    var totalBookings = await context.BookingRequests.CountAsync();

        //    return Ok(new
        //    {
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalBookings = totalBookings,
        //        Bookings = bookingList
        //    });
        //}
        public async Task<ActionResult> getAllBooking()
        {
            var bookings = await context.BookingRequests
                                 .Include(t => t.Table)
                                 .Include(u => u.User).ToListAsync();

            var bookingList = bookings.Select(b => new BookingRequestDTO
            {
                BookingId = b.BookingId,
                UserId = (int)b.UserId,
                UserName = b.User.UserName,
                Email = b.User.Email,
                TableId = (int)b.TableId,
                TableNumber = b.Table.TableNumber,
                ReservationDate = b.ReservationDate,
                NumberOfGuests = b.NumberOfGuests,
                Status = b.Status,
                Note = b.Note
            });

            var totalBookings = await context.BookingRequests.CountAsync();

            return Ok(new
            {
                Bookings = bookingList
            });
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> updateStatusBooking(int bookingId, bool isApproved)
        {
            string toEmail = "manhvv15@gmail.com";
            string subject = "Information Booking at Restaurant PRN231";
            string body = "";
            var booking = await context.BookingRequests.FirstOrDefaultAsync(b => b.BookingId == bookingId && b.Status == "pending");
            if (booking == null)
            {
                return NotFound(new { message = "No pending booking" });
            }
            if (isApproved)
            {
                var avaibilityRsult = await tableController.CheckAvailability(reservationDate) as OkObjectResult;
                booking.Status = "completed";
                body = "Dat ban thanh cong";
               // email.SendEmail(toEmail, subject, body);
                //var checkTable = await context.Tables.AnyAsync(t => t.Status == "available"); ///ngay , gio -1
                //if (checkTable)
                //{
                //    booking.Status = "completed";
                //    body = "Dat ban thanh cong";
                //    email.SendEmail(toEmail, subject, body);
                //}
            }
            else
            {
                body = "Dat ban that bai";
              //  email.SendEmail(toEmail, subject, body);
                booking.Status = "canceled";
            }
            context.BookingRequests.Update(booking);
            await context.SaveChangesAsync();

            return Ok(new { message = "Booking status updated successfully" });
        }

    }
}
