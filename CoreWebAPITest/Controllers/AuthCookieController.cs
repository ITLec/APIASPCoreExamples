using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebAPITest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthCookieController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInMgr;

        public AuthCookieController(SignInManager<IdentityUser> signInMgr)
        {
            _signInMgr = signInMgr;
        }

        [HttpPost("api/auth/login")]
        public async Task<IActionResult> Login([FromBody]
CredentialModel model)
        {
            var result = await _signInMgr.PasswordSignInAsync(model.UserName,
            model.Password, false, false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest("Failed to login");
        }
    }

    public class CredentialModel
    {
        public string UserName { get;  set; }
        public string Password { get;  set; }
    }
}