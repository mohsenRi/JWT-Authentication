using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet(template: "GetUserResponse")]
        [Authorize(Policy = Policies.User)]
        public IActionResult GetUserResponse()
        {
            return Ok("This is a response for user");
        }


        [HttpGet(template: "GetAdminResponse")]
        [Authorize(Policy = Policies.Admin)]
        public IActionResult GetAdminResponse()
        {
            return Ok("This is a response for Admin");
        }
    }
}
