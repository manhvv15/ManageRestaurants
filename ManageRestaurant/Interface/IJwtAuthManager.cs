using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManageRestaurant.Interface
{
    public interface IJwtAuthManager
    {
        string GenerateToken(string username, string password, string role);
    }

    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly IConfiguration _configuration;

        public JwtAuthManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string username, string password, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:JwtSecurityKey"]));
            //   var key = Convert.FromBase64String(_configuration["Jwt:Key"]);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));
            var expiry = DateTime.UtcNow.AddMinutes(30);
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim("Password", password),
            new Claim(ClaimTypes.Role, role)
        };

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );
            

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }

}
