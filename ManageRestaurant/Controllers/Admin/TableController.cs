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
        Email email = new Email();
        public TableController(ManageRestaurantContext context)
        {
            this.context = context;
        }
        [HttpGet("CheckTableAvailability")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> CheckAvailability(DateTime reservationDate)
        {
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
    }
}
