using ManageRestaurantsClient.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;


namespace ManageRestaurantsClient.Pages.Admin
{
    public class bookingModel : PageModel
    {
        private readonly HttpClient _httpClient = new HttpClient(); 
        public List<BookingRequestDTO> Bookings { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5000/api/BookingRequest");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                if(responseData != null)
                {
                    Bookings = JsonConvert.DeserializeObject<List<BookingRequestDTO>>(responseData);

                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
        }

    }
}
