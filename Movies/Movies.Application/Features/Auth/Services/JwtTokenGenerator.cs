using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Data.Entities;
using Movies.Application.Features.Auth.DTOs;
using Movies.Application.Settings;

namespace Movies.Application.Features.Auth.Services;

internal class JwtTokenGenerator(IOptions<JwtSettings> options) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public AuthTokenDto GenerateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.EmailVerified, user.EmailConfirmed.ToString()),
            new(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
            // new(JwtRegisteredClaimNames.Picture, "profile picutre url"), // maybe later to show users profile picutre
        ];

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var secretKey = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_jwtSettings.ExpiryMinutes);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        var authTokenDto = new AuthTokenDto
        {
            AccessToken = jwt,
            ExpiresIn = (int)(expires - now).TotalSeconds,
            TokenType = JwtBearerDefaults.AuthenticationScheme,
            RefreshToken = "needs to implement this"
        };
        
        return authTokenDto;
    }
}