using Movies.Application.Data.Entities;
using Movies.Application.Features.Auth.DTOs;

namespace Movies.Application.Features.Auth.Services;

internal interface IJwtTokenGenerator
{
     AuthTokenDto GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}