using ManageRestaurant.Interface;
using ManageRestaurant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
                //var token = _jwtAuthManager.GenerateToken(loggedInUser.Email, loggedInUser.Role);
                //return Ok(new { token, message = "Login successful." });

                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:JwtSecurityKey"]));

                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(20),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { token = tokenString, message = "Login successful." });

            }

            return Unauthorized(new { message = "Invalid username or password." });
        }
       


        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var currentUser = HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(currentUser))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            // Lấy role của người dùng từ claims
            var userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            // Tạo token mới
            var newToken = _jwtAuthManager.GenerateToken(currentUser, userRole);
            return Ok(new { token = newToken });
        }

    }

}
