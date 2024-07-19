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
using CodeMegaVNPay.Models;
using CodeMegaVNPay.Services;
using ManageRestaurant.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        private readonly IVnPayService _vnPayService;

        DateTime reservationDate;


        Email email = new Email();
        public BookingRequestController(ManageRestaurantContext context, IBookingRequestRepository bookingRequestRepository, IVnPayService vnPayService)
        {
            this.context = context;
            _bookingRequestRepository = bookingRequestRepository;
            _vnPayService = vnPayService;
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
            return Ok(bookingList);
        }

        [HttpGet("getBookingById/{userId}")]
        //[Authorize]
        public async Task<ActionResult> getBookingById(int userId)
        {
            var bookings = await context.BookingRequests.Where(b => b.UserId == userId).Include(t => t.Table).Include(u => u.User).ToListAsync();
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
            return Ok(bookingList);
        }


        [HttpPut]
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
                booking.Status = "completed";
                body = "Dat ban thanh cong";
                email.SendEmail(toEmail, subject, body);
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
                email.SendEmail(toEmail, subject, body);
                booking.Status = "canceled";
            }
            context.BookingRequests.Update(booking);
            await context.SaveChangesAsync();
            return Ok(new { message = "Booking status updated successfully" });
        }
        // table : get List , nhap gio 

        [HttpPost("AddBookingRequestAsync")]
        [Authorize]
        public async Task<ActionResult> AddBookingRequestAsync([FromBody] AddBookingRequestDTO addBookingRequestDTO)
        {
            try
            {
                string url = "";
                var result = await _bookingRequestRepository.AddBookingRequestAsync(addBookingRequestDTO);
                var toEmail = await context.Users.Where(x => x.UserId == result.UserId).Select(x => x.Email).FirstOrDefaultAsync();
                var confirmationLink = $"https://localhost:7110/home/confirmBooking?id={result.BookingId}";

                PaymentInformationModel model = new()
                {
                    OrderType = "Deposit",
                    Amount = (double)addBookingRequestDTO.depositAmount,
                    OrderDescription = "Pauu Restaurant", //booking Id
                    Name = addBookingRequestDTO.UserId.ToString()
                };
                if (addBookingRequestDTO.IsDeposited == true && addBookingRequestDTO.depositAmount != null)
                {
                    url = paymentMethod(model);
                }
                var subject = "Booking Confirmation";
                var body = $"Dear Customer,\n\nPlease click on the following link to confirm your booking: {confirmationLink}\n\nThank you.";
                if (toEmail != null)
                {
                    var emailService = new Email();
                    emailService.SendEmail(toEmail, subject, body);
                }
                return Ok(new
                {
                    result = result,
                    url = url
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        
        }
        private string paymentMethod(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return url;
        }

        [HttpGet("PaymentCallback")]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            var result = "";
            if (response.Success)
            {
                result = "Deposit sucessfully";
            }
            else
            {
                result = "Deposit unsucessfully";
            }
            return Ok(result);
        }
        [HttpPost("getTableByNumOfPeople")]
        [Authorize]

        public async Task<ActionResult> getTableByNumOfPeople([FromBody] int numberOfPeople)
        {
            var lstTable = new List<Models.Table>();
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
    }
}
