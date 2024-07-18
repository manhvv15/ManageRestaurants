using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageRestaurantsClient.Pages.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class userModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
