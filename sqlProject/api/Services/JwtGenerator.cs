using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Services;

public class JwtGenerator : IJwtGenerator
{
    private readonly IConfiguration _configuration;
    private readonly string _jwtKey;

    public JwtGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("id", user.Id.ToString()),
            new Claim("firstname", user.FirstName),
            new Claim("lastname", user.LastName),
            new Claim("email", user.Email),
        };
        
        if (string.IsNullOrEmpty(_jwtKey))
            throw new InvalidOperationException("JWT_KEY environment variable is not set.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: signIn
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}