using ManageRestaurantsClient.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ManageRestaurantsClient.Pages.Users
{
    public class RegisterModel : PageModel
    {
        HttpClient httpClient = new HttpClient();
        [BindProperty]
        public UserDTO userDTO { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {

            var url = $"https://localhost:5000/api/Login/register?userName={userDTO.UserName}&Email={userDTO.Email}&password={userDTO.Password}";
            HttpResponseMessage response = await httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserDTO>(responseString);
                //var user =
                TempData["Message"] = "Đăng ký thành công";
                return RedirectToPage("/Users/Login");
            }
            else
            {
                TempData["Error"] = "Đăng ký thất bại";
                return Page();
            }

        }

    }
}
