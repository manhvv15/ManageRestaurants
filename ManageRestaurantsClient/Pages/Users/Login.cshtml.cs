using ManageRestaurantsClient.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;

namespace ManageRestaurantsClient.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LoginModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public UserDTO User { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var url = $"https://localhost:5000/api/Login/login?Email={User.Email}&password={User.Password}";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserDTO>(responseString);

                // Lưu token vào cookie
                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Role, result.Role)
                };

                Response.Cookies.Append("UserId", result.UserId.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(1)
                });
                Response.Cookies.Append("Role", result.Role.ToString(), new CookieOptions
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

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1), // Thời gian hết hạn của cookie
                    IsPersistent = true, // Cookie có lưu lại khi đóng trình duyệt không
                    AllowRefresh = true // Cho phép cập nhật lại cookie
                };
                TempData["Message"] = result.Message;
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (result.Role == "Admin" || result.Role == "Staff")
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
