using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using static ManageRestaurantsClient.DTO.BookingRequestDTO;
using System.Net.Http;

namespace ManageRestaurantsClient.Pages.Users
{
    public class VNPay_ResponseModel : PageModel
    {
        
        private readonly HttpClient _httpClient = new HttpClient();
        public async Task OnGetAsync()
        {
            try
            {
                //int userId = Convert.ToInt32(Request.Cookies["UserId"]);
                //var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5000/api/BookingRequest/PaymentCallBack");
                //var response = await _httpClient.SendAsync(request);
                //response.EnsureSuccessStatusCode();
                //var responseData = await response.Content.ReadAsStringAsync();
                //var response = _vnPayService.PaymentExecute(Request.Query);
                //var result = "";
                //if (response.Success)
                //{
                //    result = "Deposit sucessfully";
                    ViewData["depositStatus"] = "Deposit sucessfully";
                //}
                //else
                //{
                //    result = "Deposit unsucessfully";
                //}
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
