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
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> CheckAvailability(DateTime reservationDate)
        {
            if (reservationDate != null)
            {
                reservaDate = reservationDate;

            }

            TotalTables = context.Tables.Count();
            // Lấy danh sách các bàn đã được đặt vào ngày đó
            var reservedTables = await context.BookingRequests
         .Where(b => b.ReservationDate == reservationDate)
         .Select(b => b.TableId)
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
                int? selectedTableId = await GetAvailableTableId(booking.ReservationDate);
                if (selectedTableId.HasValue)
                {
                    UpdateBookingWithTable(booking, selectedTableId.Value);
                    body = "Dat ban thanh cong";
                    //email.SendEmail(toEmail, subject, body);
                }
                //var availabilityResult = await CheckAvailability(booking.ReservationDate) as OkObjectResult;

                //int idTableSelect = 1;
                //if (availabilityResult == null)
                //{
                //    return BadRequest(new { message = "Error checking table availability" });
                //}

                //var availableTables = ((IEnumerable<dynamic>)availabilityResult.Value)
                //    .Where(t => t.IsAvailable)
                //    .ToList();

                //if (availableTables.Count > 0)
                //{
                //    int selectedTableId = availableTables.First().TableId;
                //    booking.TableId = selectedTableId;
                //    booking.Status = "completed";
                //    body = "Dat ban thanh cong";
                //    // email.SendEmail(toEmail, subject, body);
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
        private async Task<int?> GetAvailableTableId(DateTime reservationDate)
        {
            var availabilityResult = await CheckAvailability(reservationDate) as OkObjectResult;
            if (availabilityResult == null)
            {
                return null;
            }

            var availableTables = ((IEnumerable<dynamic>)availabilityResult.Value)
                .Where(t => t.IsAvailable)
                .ToList();

            if (availableTables.Count > 0)
            {
                return availableTables.First().TableId;
            }

            return null;
        }

        private void UpdateBookingWithTable(BookingRequest booking, int tableId)
        {
            booking.TableId = tableId;
            booking.Status = "completed";
        }
    }
}
