using ManageRestaurant.Interface;
using ManageRestaurant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManageRestaurant.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IConfiguration configuration;

        public LoginController(IUsersRepository usersRepository, IJwtAuthManager jwtAuthManager, IConfiguration configuration)
        {
            _usersRepository = usersRepository;
            _jwtAuthManager = jwtAuthManager;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string userName, string Email, string password)
        {
            if (await _usersRepository.RegisterUser(userName,Email, password))
                return Ok(new { message = "User registered successfully." });

            return BadRequest(new { message = "User already exists." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string Email, string password)
        {
            var loggedInUser = await _usersRepository.LoginUser(Email, password);

            if (loggedInUser != null)
            {
                var token = _jwtAuthManager.GenerateToken(loggedInUser.Email, loggedInUser.Role);
                //return Ok(new { token, message = "Login successful." });
                return Ok(new { userId = loggedInUser.UserId,role = loggedInUser.Role, token, message = "Login successful." });

            }

            return Unauthorized(new { message = "Invalid username or password." });
        }
        [HttpGet("user-info/{userId}")]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            var user = await _usersRepository.GetUserById(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var userInfo = new
            {
                user.UserId,
                user.UserName,
                user.Email,
                user.Balance,
                user.Phone,
                user.Role,
                user.CreatedAt,
                user.CreatedBy,
                user.UpdateAt,
                user.UpdateBy,
                user.DeletedAt,
                user.DeletedBy
            };

            return Ok(userInfo);
        }


        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            // Ensure the current user is authenticated
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            // Get the email and role of the current user from claims
            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            // Generate a new token
            var newToken = _jwtAuthManager.GenerateToken(userEmail, userRole);

            // Return the new token in the response
            return Ok(new { token = newToken });
        }


    }

}
