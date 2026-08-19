using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Models;

namespace ToDoList.Security
{
    public class JwtTokenGenerator
    {
        private readonly string key;
        private readonly string issuer;
        private readonly string audience;

        public JwtTokenGenerator(string key, string issuer, string audience)
        {
            this.key = key;
            this.issuer = issuer;
            this.audience = audience;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}