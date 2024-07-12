using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageRestaurantsClient.Pages.Users
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToPage("/Users/Login");
        }
    }
}
