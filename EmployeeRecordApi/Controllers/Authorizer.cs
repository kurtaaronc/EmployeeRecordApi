using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeRecordApi.Models;

namespace EmployeeRecordApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Authorizer : ControllerBase
    {
        private const string SecretToken = "needToStoreThisInSecureServices";
        private static readonly TimeSpan TokenLife = TimeSpan.FromHours(1);

        [HttpPost("/GenerateToken")]
        public IActionResult GenerateToken([FromBody] TokenGeneration request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretToken);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, request.Email),
                new Claim(JwtRegisteredClaimNames.Email, request.Email),
                new Claim("userId", request.UserId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLife),
                Issuer = "flexiSourceIt",
                Audience = "kurtAaronCabrera",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("needToStoreThisInSecureServices")),SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });
        }
    }
}
