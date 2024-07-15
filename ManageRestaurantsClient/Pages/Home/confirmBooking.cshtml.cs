using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageRestaurantsClient.Pages.Home
{
    public class confirmBookingModel : PageModel
    {
        public int? BookingId { get; set; }

        public void OnGet(int? id)
        {
            BookingId = id;
        }
    }
}
