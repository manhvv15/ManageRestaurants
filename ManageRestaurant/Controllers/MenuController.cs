using ManageRestaurant.DTO;
using ManageRestaurant.Helper;
using ManageRestaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly ManageRestaurantContext context;
        public MenuController(ManageRestaurantContext context)
        {
            this.context = context;
        }

        [HttpGet("getMenu")]
        [Authorize]
        
        public async Task<ActionResult<Menu>> GetAllMenuItems()
        {
            var menuItems = await context.Menus
                .Select(mi => new MenuDTO
                {
                    Name = mi.Name,
                    Address = mi.Restaurant.Address,
                    Phone = mi.Restaurant.Phone,
                    Email = mi.Restaurant.Email,
                    Description = mi.Description,
                    Rating = (double)mi.Restaurant.Rating
                })
                .ToListAsync();

            return Ok(menuItems);
        }


        //[Authorize(Roles = AppRole.User)]
        //[HttpGet("test")]
        //public IActionResult test()
        //{
        //    return Ok();
        //}

    }
}
