using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoreWebAPITest.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class AuthController : ControllerBase
    {
      //  private readonly IConfigurationRoot _config;
        public AuthController()
        {
        }
        [HttpPost("api/auth/token")]
        public IActionResult CreateToken([FromBody] CredentialsModel model)
        {
            if (model == null)
            {
                return BadRequest("Request is Null");
            }
            var findusr = new CredentialsModel() { UserName = "1", Password = "1" };
            if (findusr != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, findusr.UserName),

new Claim(JwtRegisteredClaimNames.Sub, findusr.UserName),
new Claim(JwtRegisteredClaimNames.Jti,
Guid.NewGuid().ToString()),
};
                var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("this is my custom Secret key for authnetication"));
                var creds = new SigningCredentials(key,
                SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                issuer: "Tokens:Issuer",
                audience: "Tokens:Audience",
                claims: claims,
expires: DateTime.UtcNow.AddMinutes(12),
signingCredentials: creds
);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return BadRequest("Failed to generate Token");
        }


    }
}