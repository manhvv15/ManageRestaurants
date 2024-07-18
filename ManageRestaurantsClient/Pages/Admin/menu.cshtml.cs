using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageRestaurantsClient.Pages.Admin
{
    [Authorize]
    public class menuModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
