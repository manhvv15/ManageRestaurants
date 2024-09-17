using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using ManageRestaurantsClient.DTO;

namespace ManageRestaurantsClient.Pages.Admin
{
    public class tableModel : PageModel
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public List<TableStatus> Tables { get; set; }
        [BindProperty]
        public DateTime ReservationDate { get; set; } 
        [BindProperty]
        public string ReservationTime { get; set; } 
        public async Task OnGetAsync()
        {
            await FetchTableStatuses();
        }

        public async Task OnPostAsync()
        {
            await FetchTableStatuses();
        }
        public async Task FetchTableStatuses()
        {
            try
            {

                var token = Request.Cookies["AuthToken"];
                var reservationDateTime = ReservationDate.ToString("yyyy-MM-dd") + " " + ReservationTime;
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:5000/api/Table/CheckTableAvailability?reservationDate={reservationDateTime}");
                request.Headers.Add("Authorization", "Bearer " + token);
                // Retrieve the token from cookies
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();

                if (responseData != null)
                {
                    Tables = JsonConvert.DeserializeObject<List<TableStatus>>(responseData);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
