using ManageRestaurant.DTO;
using ManageRestaurant.Helper;
using ManageRestaurant.Interface;
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
        private readonly IMenuRepository _menuRepository;

        public MenuController(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        [HttpGet("GetMenusAsync")]
        [Authorize]
        
        public async Task<ActionResult> GetMenusAsync()
        {
            var menuItems = await _menuRepository.GetMenusAsync();
            return Ok(menuItems);
        }

        [HttpPost("GetMenuByIdAsync")]
        [Authorize(Roles = AppRole.Admin)]

        public async Task<ActionResult> GetMenuByIdAsync([FromBody] int id)
        {
            var result = await _menuRepository.GetMenuByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("AddMenuAsync")]
        [Authorize(Roles = AppRole.Admin)]

        public async Task<ActionResult> AddMenuAsync([FromBody] Menu menu)
        {
            var result = await _menuRepository.AddMenuAsync(menu);
            return Ok(result);
        }

        [HttpPost("UpdateMenuAsync")]
        [Authorize(Roles = AppRole.Admin)]

        public async Task<ActionResult> UpdateMenuAsync(int id, [FromBody] MenuDTO menu)
        {
            var result = await _menuRepository.UpdateMenuAsync(id ,menu);
            return Ok(result);
        }

        [HttpPost("DeleteMenuAsync")]
        [Authorize(Roles = AppRole.Admin)]

        public async Task<ActionResult> DeleteMenuAsync([FromBody] int id)
        {
            var result = await _menuRepository.DeleteMenuAsync(id);
            return Ok(result);
        }

    }
}
