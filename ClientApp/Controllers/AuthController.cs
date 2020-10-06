using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ClientApp.Models;

namespace ClientApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var client = _httpClientFactory.CreateClient("ClientApp");
            var jsonBody = JsonConvert.SerializeObject(login);
            var content = new StringContent(jsonBody,Encoding.UTF8,"application/json");
            var response = client.PostAsync("/Api/Auth/Login", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var token = response.Content.ReadAsStringAsync().Result;
                var values = JsonConvert.DeserializeObject<User>(token);
                var claims=new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,login.UserName),
                    new Claim(ClaimTypes.Name,login.UserName),
                    new Claim(ClaimTypes.Role,values.RoleName),
                    new Claim("AccessToken",values.Token)
                };
                var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                var principal=new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                };
                HttpContext.SignInAsync(principal, properties);
                return Redirect("/Home");
            }
            else
            {
                ModelState.AddModelError("Username","User Not Valid");
                return View(login);
            }

        }
    }
}