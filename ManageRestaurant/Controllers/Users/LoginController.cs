﻿using ManageRestaurant.Interface;
using ManageRestaurant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManageRestaurant.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IJwtAuthManager _jwtAuthManager;

        public LoginController(IUsersRepository usersRepository, IJwtAuthManager jwtAuthManager)
        {
            _usersRepository = usersRepository;
            _jwtAuthManager = jwtAuthManager;
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
                var token = _jwtAuthManager.GenerateToken(loggedInUser.UserName, loggedInUser.Role);
                return Ok(new { token, message = "Login successful." });
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
