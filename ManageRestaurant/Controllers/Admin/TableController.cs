using ManageRestaurant.Models;
using ManageRestaurant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManageRestaurant.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        public int TotalTables = 8;
        private readonly ManageRestaurantContext context;
        DateTime reservaDate;
        Email email = new Email();
        public TableController(ManageRestaurantContext context)
        {
            this.context = context;
            //  this.reservaDate = reservationDate;
        }
        [HttpGet("CheckTableAvailability")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CheckAvailability(DateTime reservationDate)
        {
            if (reservationDate != null)
            {
                reservaDate = reservationDate;

            }

            TotalTables = context.Tables.Count();
            // Lấy danh sách các bàn đã được đặt vào ngày đó
            var reservedTables = await context.Reserveds
        .Where(r => r.ReservedDate.Date == reservationDate.Date)
        .Select(r => r.TableId)
        .ToListAsync();

            // Tạo danh sách tất cả các bàn
            var allTables = Enumerable.Range(1, TotalTables).ToList();

            // Lấy danh sách các bàn trống
            var tableStatuses = Enumerable.Range(1, TotalTables)
         .Select(tableId => new
         {
             TableId = tableId,
             IsAvailable = !reservedTables.Contains(tableId)
         })
         .ToList();

            return Ok(tableStatuses);
        }
        List<int> ListIdAvai = new List<int>();
        [HttpPut("updateStatusBooking")]
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
                var tableListID = await GetAvailableTableId(booking.ReservationDate);
                return Ok(new { tableListID = tableListID });

            }
            else
            {
                body = "Dat ban that bai";
                //  email.SendEmail(toEmail, subject, body);
                //body = "Dat ban thanh cong";
                //    //email.SendEmail(toEmail, subject, body);
                booking.Status = "canceled";
                context.BookingRequests.Update(booking);
                await context.SaveChangesAsync();
                return Ok(new { message = "Booking is canceled" });

            }
        }
        [HttpPut("updateApproveTable")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateApproveTable(int tableId, DateTime reservationDate)
        {
            var booking = await context.BookingRequests.FirstOrDefaultAsync(b => b.ReservationDate == reservationDate);
            if (booking == null)
            {
                return NotFound(new { message = "No pending booking found for the specified table ID and reservation date" });
            }
            var reserved = await context.Reserveds
         .FirstOrDefaultAsync(b => b.ReservedDate == reservationDate);
            var newReservation = new Reserved
            {
                TableId = tableId,
                ReservedDate = reservationDate,
                BookingId = booking.BookingId
            };
            context.Reserveds.Add(newReservation);
            booking.Status = "completed";
            // string toEmail = booking.CustomerEmail; // Assuming you have the customer's email
            // string subject = "Booking Confirmation";
            // string body = "Your booking has been successfully completed.";
            // email.SendEmail(toEmail, subject, body);
            await context.SaveChangesAsync();

            return Ok(new { message = "Booking status updated to completed" });
        }
        [HttpGet("GetAvailableTableId")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<int>>> GetAvailableTableId(DateTime reservationDate)
        {
            var availabilityResult = await CheckAvailability(reservationDate) as OkObjectResult;
            if (availabilityResult == null)
            {
                return NotFound();
            }

            var availableTables = ((IEnumerable<dynamic>)availabilityResult.Value)
                .Where(t => t.IsAvailable)
                .ToList();

            List<int> listTableId = new List<int>();
            if (availableTables.Count > 0)
            {
                foreach (var table in availableTables)
                {
                    listTableId.Add(table.TableId);
                }
            }

            return Ok(listTableId);
        }

        private void UpdateBookingWithTable(BookingRequest booking, int tableId)
        {
            booking.TableId = tableId;
            booking.Status = "completed";
        }
    }
}
