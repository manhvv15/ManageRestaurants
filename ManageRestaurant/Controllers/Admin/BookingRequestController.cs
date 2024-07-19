using ManageRestaurant.DTO;
using ManageRestaurant.Helper;
using ManageRestaurant.Interface;
using ManageRestaurant.Models;
using ManageRestaurant.Repository;
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
        private readonly ManageRestaurantContext context;
        private readonly TableController tableController;
        private readonly IBookingRequestRepository _bookingRequestRepository;
        DateTime reservationDate;

        Email email = new Email();
        public BookingRequestController(ManageRestaurantContext context, IBookingRequestRepository bookingRequestRepository)
        {
            this.context = context;
            _bookingRequestRepository = bookingRequestRepository;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
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

        // table : get List , nhap gio 

        [HttpPost("AddBookingRequestAsync")]
        [Authorize]

        public async Task<ActionResult> AddBookingRequestAsync([FromBody] AddBookingRequestDTO addBookingRequestDTO)
        {
            try
            {
                var result = await _bookingRequestRepository.AddBookingRequestAsync(addBookingRequestDTO);
                var toEmail = await context.Users.Where(x => x.UserId == result.UserId).Select(x => x.Email).FirstOrDefaultAsync();
                var confirmationLink = $"https://localhost:7110/home/confirmBooking?id={result.BookingId}";

                var subject = "Booking Confirmation";
                var body = $"Dear Customer,\n\nPlease click on the following link to confirm your booking: {confirmationLink}\n\nThank you.";
                if (toEmail != null)
                {
                    var emailService = new Email();
                    emailService.SendEmail(toEmail, subject, body);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("getTableByNumOfPeople")]
        [Authorize]

        public async Task<ActionResult> getTableByNumOfPeople([FromBody] int numberOfPeople)
        {
            var lstTable = new List<Table>();
            if (numberOfPeople > 0)
            {
                lstTable = await context.Tables.Where(x => x.Capacity == numberOfPeople).ToListAsync();
            }
            else
            {
                lstTable = await context.Tables.ToListAsync();
            }
            return Ok(lstTable);
        }

        [HttpPost("ConfirmBooking")]
        public async Task<ActionResult> ConfirmBooking([FromQuery] int id)
        {
            try
            {
                var booking = await _bookingRequestRepository.GetBookingByIdAsync(id);

                if (booking == null)
                {
                    return NotFound();
                }

                booking.Status = "pending";
                await _bookingRequestRepository.UpdateBookingAsync(booking);

                return Ok("Booking confirmed successfully!");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "An error occurred while processing your request.");
            }
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
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetBookingStatistics()
        {
            var now = DateTime.Now;
            var startOfToday = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var todayBookings = await context.BookingRequests
                                             .Where(b => b.ReservationDate >= startOfToday)
                                             .ToListAsync();

            var lastWeekBookings = await context.BookingRequests
                                                .Where(b => b.ReservationDate >= startOfWeek && b.ReservationDate < startOfToday)
                                                .ToListAsync();

            var lastMonthBookings = await context.BookingRequests
                                                 .Where(b => b.ReservationDate >= startOfMonth && b.ReservationDate < startOfToday)
                                                 .ToListAsync();

            var statistics = new
            {
                Today = new
                {
                    New = todayBookings.Count(b => b.Status == "new"),
                    Pending = todayBookings.Count(b => b.Status == "pending"),
                    Cancelled = todayBookings.Count(b => b.Status == "cancelled"),
                    Completed = todayBookings.Count(b => b.Status == "completed")
                },
                LastWeek = new
                {
                    New = lastWeekBookings.Count(b => b.Status == "new"),
                    Pending = lastWeekBookings.Count(b => b.Status == "pending"),
                    Cancelled = lastWeekBookings.Count(b => b.Status == "cancelled"),
                    Completed = lastWeekBookings.Count(b => b.Status == "completed")
                },
                LastMonth = new
                {
                    New = lastMonthBookings.Count(b => b.Status == "new"),
                    Pending = lastMonthBookings.Count(b => b.Status == "pending"),
                    Cancelled = lastMonthBookings.Count(b => b.Status == "cancelled"),
                    Completed = lastMonthBookings.Count(b => b.Status == "completed")
                }
            };

            return Ok(statistics);
        }
    }
}
