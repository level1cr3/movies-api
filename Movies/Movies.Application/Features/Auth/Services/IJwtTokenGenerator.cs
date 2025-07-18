using Movies.Application.Data.Entities;
using Movies.Application.Features.Auth.DTOs;

namespace Movies.Application.Features.Auth.Services;

internal interface IJwtTokenGenerator
{
     Task<AuthTokenDto> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles);
}