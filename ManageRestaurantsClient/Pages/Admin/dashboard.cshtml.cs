using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace ManageRestaurantsClient.Pages.Admin
{
    public class dashboardModel : PageModel
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public JObject BookingStatistics { get; set; }
        public async Task OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];             
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5000/api/BookingRequest/statistics");
            request.Headers.Add("Authorization", "Bearer " + token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseData))
            {
                BookingStatistics = JObject.Parse(responseData);
            }
        }
    }
}
