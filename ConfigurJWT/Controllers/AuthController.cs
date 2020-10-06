using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWTAuthentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private List<User> _appUsers = new List<User>
        {
            new User {  FullName = "Mohsen Rafiei",  UserName = "admin", Password = "1234", RoleName = "Admin" },
            new User {  FullName = "Test User",  UserName = "user", Password = "1234", RoleName = "User" }
        };
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost(template: "Login")]
        public IActionResult Login([FromBody] User user)
        {
            var validUser = ValidateUser(user);
            if (validUser == null)
            {
                return Unauthorized();
            }
            else
            {
                var token = GenerateJwtToken(validUser);
                return Ok(new
                {
                    userInfo=validUser,
                    token=token
                });
            }
            
        }

        private User ValidateUser(User user)
        {
            var userInfo = _appUsers.SingleOrDefault(p => p.UserName == user.UserName && p.Password == user.Password);
            //if can't find user return null
            return userInfo;
        }

       private string GenerateJwtToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim("fullName", userInfo.FullName.ToString()),
                new Claim("role",userInfo.RoleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
