using FeedBack_System.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FeedBack_System.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration, IMongoCollection<User> users)
        {
            _configuration = configuration;
            _users = users;
        }

        public async Task<AuthResponse> AuthenticateAsync(LoginRequest loginRequest)
        {
            var user = await _users.Find(u => u.Username == loginRequest.Username).FirstOrDefaultAsync();

            if (user == null || user.Password != loginRequest.Password)
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                User = user.Username,
                Token = token,
                Role = user.Role
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
