using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Data.Entities;
using Movies.Application.Data.Repositories;
using Movies.Application.Data.Repositories.JwtRefreshToken;
using Movies.Application.Features.Auth.DTOs;
using Movies.Application.Settings;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Services;

internal class JwtTokenGenerator(
    IOptions<JwtSettings> options,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IRequestContextService requestContextService,
    ILogger<JwtTokenGenerator> logger)
    : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public async Task<AuthTokenDto> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles)
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

        var (refreshToken, refreshTokenId) = await GenerateRefreshTokenAsync(user.Id);
        
        var authTokenDto = new AuthTokenDto
        {
            AccessToken = jwt,
            ExpiresIn = (int)(expires - now).TotalSeconds,
            TokenType = JwtBearerDefaults.AuthenticationScheme,
            RefreshToken = refreshToken,
            RefreshTokenId = refreshTokenId
        };

        return authTokenDto;
    }

    private async Task<(string refreshToken, Guid refreshTokenId)> GenerateRefreshTokenAsync(Guid userId)
    {
         var token= Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
         var now = DateTime.UtcNow;
         
         var refreshToken = new RefreshToken
         {
             Token = token,
             CreatedByIp = requestContextService.GetClientIp(),
             CreatedAt = now,
             ExpiresAt = now.AddDays(_jwtSettings.RefreshTokenExpiryDays),
             UserId = userId,
         };
         
         refreshTokenRepository.Create(refreshToken);
         var rowsAffected = await unitOfWork.SaveChangesAsync();

         if (rowsAffected < 1)
         {
             logger.LogError("Failed to save refresh token for user {UserId}", userId);
             return (string.Empty, Guid.Empty);
         }
         
         return (token, refreshToken.Id);
         
         // Note : If the return ever grows, refactor to an object easily.
         // For now: Stick with the tuple!
    }
    
    
    
}