using ManageRestaurantsClient.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace ManageRestaurantsClient.Pages.Users
{
    public class BookingHistoryModel : PageModel
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public List<BookingRequestDTO.Booking> Bookings { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                int userId = Convert.ToInt32(Request.Cookies["UserId"]);
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5000/api/BookingRequest/getBookingById/" + userId);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                if (responseData != null)
                {
                    Bookings = JsonConvert.DeserializeObject<List<BookingRequestDTO.Booking>>(responseData);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
