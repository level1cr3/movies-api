using Movies.Application.Data.Entities;

namespace Movies.Application.Features.Auth.Services;

internal interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}