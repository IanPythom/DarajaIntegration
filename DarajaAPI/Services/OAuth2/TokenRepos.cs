using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MailServiceAPI.Services.OAuth2
{
    public class TokenRepos : ITokenRepos
    {
        private readonly IConfiguration _configuration;

        public TokenRepos(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJWT(IdentityUser user, List<string> roles)
        {
            //Create claims
            var claims = new List<Claim>(); // From System.Security.Claims

            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in roles) // Create Claim from roles
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}