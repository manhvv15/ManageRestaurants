using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageRestaurantsClient.Pages.Home
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string Message { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }
    }
}
