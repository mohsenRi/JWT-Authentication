using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ConfigurJWT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
       

        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("token")]
        public IActionResult Get()
        {
            var basicToken = HttpContext.Request.Headers["Authorization"].ToString();
            if (!basicToken.StartsWith("Basic"))
            {
                return BadRequest();
            }

            var decodedAuthToken = Encoding.UTF8.GetString(Convert.FromBase64String(basicToken.Substring(6).Trim()));
            var usernamePassword = decodedAuthToken.Split(":");
            if (usernamePassword[0]=="admin" && usernamePassword[1]=="pass")
            {
                //claim
                var token = GenerateToken(usernamePassword[0]);
                return Ok(token);

            }

            return BadRequest("error");
        }

        private string GenerateToken(string username)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2CD67028EA37422CADE7EDFEF5236E42"));

            var signinCredential = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(issuer: "http://localhost:63206/",
                audience: "http://localhost:63206/",
                claims: new[] {new Claim(ClaimTypes.Name, username)},
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signinCredential); 
            return securityToken.ToString();
        }
    }
}
