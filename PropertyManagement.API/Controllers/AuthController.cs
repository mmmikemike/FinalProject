using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PropertyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController (AppDbContext db, IConfiguration config) : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = db.Users.SingleOrDefault(u => u.Username == request.Username);

            if (user == null) // if user not found
            {
                return Unauthorized(new { message = "User Not Found" });
            }

            var token = GenerateToken(user.Username, user.Role); //generate instance token using username and role
            return Ok(new { token });
        }

        private string GenerateToken(string username, string role) //generate JWT token for the authenticated user
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken( 
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: [
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)  
                    ],
                    expires: DateTime.Now.AddHours(1), //expires in 1 hour
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
