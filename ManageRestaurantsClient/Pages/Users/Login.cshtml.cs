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
            var json = JsonConvert.SerializeObject(User);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("https://localhost:5000/api/Login/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserDTO>(responseString);
                User.Message = result.Message;
                return RedirectToPage("home/Index"); 
            }
            else
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserDTO>(responseString);
                User.Message = result.Message;
                return Page();
            }
        }
    }
}
