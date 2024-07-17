using ManageRestaurant.DTO;
using ManageRestaurant.Models;
using ManageRestaurant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.XSSF.UserModel;

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
        [HttpGet("exportExcel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportBookingsToExcel()
        {
            var bookings = await context.BookingRequests
                .Include(t => t.Table)
                .Include(u => u.User)
                .ToListAsync();

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Bookings");
            var headerRow = sheet.CreateRow(0);

            var headers = new string[] { "BookingId", "UserId", "UserName", "Email", "TableId", "TableNumber", "ReservationDate", "NumberOfGuests", "Status", "Note" };
            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.CreateCell(i).SetCellValue(headers[i]);
            }

            for (int i = 0; i < bookings.Count; i++)
            {
                var row = sheet.CreateRow(i + 1);
                var booking = bookings[i];

                row.CreateCell(0).SetCellValue(booking.BookingId);
                row.CreateCell(1).SetCellValue(booking.UserId ?? 0);
                row.CreateCell(2).SetCellValue(booking.User.UserName ?? string.Empty);
                row.CreateCell(3).SetCellValue(booking.User.Email);
                row.CreateCell(4).SetCellValue(booking.TableId ?? 0);
                row.CreateCell(5).SetCellValue(booking.Table.TableNumber);
                row.CreateCell(6).SetCellValue(booking.ReservationDate.ToString("yyyy-MM-dd HH:mm:ss"));
                row.CreateCell(7).SetCellValue(booking.NumberOfGuests);
                row.CreateCell(8).SetCellValue(booking.Status);
                row.CreateCell(9).SetCellValue(booking.Note);
            }
            using (var exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                var bytes = exportData.ToArray();

                var fileName = "Bookings.xlsx";
                Response.Headers["Content-Disposition"] = $"attachment; filename={fileName}";

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
