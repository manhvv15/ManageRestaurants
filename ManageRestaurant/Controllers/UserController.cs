using ManageRestaurant.DTO;
using ManageRestaurant.Helper;
using ManageRestaurant.Interface;
using ManageRestaurant.Models;
using ManageRestaurant.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManageRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;

        public UserController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpPost("GetUserById")]
        [Authorize]

        public async Task<ActionResult> GetUserById([FromBody]int userId)
        {
            var result = await _usersRepository.GetUserById(userId);
            return Ok(result);
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _usersRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpPost("CreateUser")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult<int>> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await _usersRepository.CreateAsync(user);
            return Ok(userId);
        }

        // POST api/users/{userId} (Update)
        [HttpPost("UpdateUser/{userId}")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult> UpdateUser(int userId, [FromBody] UserDTO userDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usersRepository.UpdateAsync(userId,userDTO);
            if (!result)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("DeleteUser")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult> DeleteUser([FromBody] int userId)
        {
            var result = await _usersRepository.DeleteAsync(userId);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
