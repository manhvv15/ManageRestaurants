using ManageRestaurantsClient.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using static ManageRestaurantsClient.DTO.BookingRequestDTO;



namespace ManageRestaurantsClient.Pages.Admin
{
    public class bookingModel : PageModel
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public List<BookingRequestDTO.BookingList> BookingList { get; set; }
        public List<BookingRequestDTO.Booking> Bookings { get; set; } = new List<BookingRequestDTO.Booking>();
        public List<int> AvailableTableIds { get; set; } = new List<int>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public class PagedResult<T>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalBookings { get; set; }
            public List<T> Bookings { get; set; }

        }
        //public async Task OnGetAsync(int? page)
        //{
        //    try
        //    {
        //        int pageNumber = page ?? 1;
        //        int pageSize = 10;
        //        var token = Request.Cookies["AuthToken"];
        //        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5000/api/BookingRequest");
        //        request.Headers.Add("Authorization", "Bearer " + token);
        //        var response = await _httpClient.SendAsync(request);
        //        response.EnsureSuccessStatusCode();
        //        var responseData = await response.Content.ReadAsStringAsync();
        //        if (responseData != null)
        //        {
        //            var result = JsonConvert.DeserializeObject<PagedResult<BookingRequestDTO>>(responseData);
        //            Bookings = result.Bookings;
        //            CurrentPage = result.PageNumber;
        //            TotalPages = (int)System.Math.Ceiling(result.TotalBookings / (double)pageSize);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        public async Task OnGetAsync()
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5000/api/BookingRequest");
                request.Headers.Add("Authorization", "Bearer " + token);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                if (responseData != null)
                {
                    BookingRequestDTO.BookingList BookingList = JsonConvert.DeserializeObject<BookingRequestDTO.BookingList>(responseData);
                    Bookings = BookingList.Bookings.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<IActionResult> OnPostApproveAsync(int bookingId)
        {
            DateTime d = new DateTime(2024, 7, 21, 19, 0, 0);
            return await GetAvailableTableId(d);
        }
        private async Task<IActionResult> GetAvailableTableId(DateTime reservationDate)
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:5000/api/Table/GetAvailableTableId?reservationDate={reservationDate}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                if (responseData != null)
                {
                    AvailableTableIds = JsonConvert.DeserializeObject<List<int>>(responseData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Error"] = "An error occurred while updating the booking status.";
            }

            return RedirectToPage("/Admin/Booking");
        }

        public async Task<IActionResult> OnPostCancelAsync(int bookingId)
        {
            return await ChangeBookingStatusAsync(bookingId, false);
        }

        private async Task<IActionResult> ChangeBookingStatusAsync(int bookingId, bool isApproved)
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:5000/api/BookingRequest?bookingId={bookingId}&isApproved={isApproved}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response != null)
                {
                    //booking.Status = isApproved ? "approved" : "canceled";
                    TempData["Success"] = "Xử lý thành công";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Error"] = "An error occurred while updating the booking status.";
            }

            return RedirectToPage("/Admin/Booking");
        }
        public async Task<IActionResult> OnPostExportExcelAsync()
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:5000/api/BookingRequest/exportExcel");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response != null)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var fileName = response.Content.Headers.ContentDisposition.FileName.Trim('"');
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Error"] = "An error occurred while updating the booking status.";
            }

            return RedirectToPage("/Admin/Booking");
        }
    }
}
