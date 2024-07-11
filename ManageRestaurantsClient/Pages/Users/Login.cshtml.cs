using ManageRestaurantsClient.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using Newtonsoft.Json;


namespace ManageRestaurantsClient.Pages.Users
{
    public class LoginModel : PageModel
    {
        HttpClient httpClient = new HttpClient();

        [BindProperty]
        public UserDTO User { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var url = $"https://localhost:5000/api/Login/login?Email={User.Email}&password={User.Password}";
                HttpResponseMessage response = await httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<UserDTO>(responseString);
                    TempData["Message"] = result.Message;
                    return RedirectToPage("/Home/Index");
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<UserDTO>(responseString);
                    TempData["Message"] = result.Message;
                    return Page();
                }
            }
        }
    }
}
