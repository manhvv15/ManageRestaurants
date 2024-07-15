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
                    // Lưu token vào cookie
                    Response.Cookies.Append("UserId", result.UserId.ToString(), new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddHours(1)
                    });
                    Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddHours(1)
                    });
                    //var user =
                    TempData["Message"] = result.Message;
                    if (result.Role == "Admin")
                    {
                        return RedirectToPage("/Admin/home");
                    }
                    else if (result.Role == "User")
                    {
                        return RedirectToPage("/Home/Index");
                    }
                    else
                    {
                        return Page();
                    }
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<UserDTO>(responseString);
                    TempData["Error"] = result.Message;
                    return Page();
                }
            }
        }
    }
}
