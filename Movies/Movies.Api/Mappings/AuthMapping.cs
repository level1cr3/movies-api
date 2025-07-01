using Movies.Application.DTOs.Auth;
using Movies.Contracts.Requests.Auth;

namespace Movies.Api.Mappings;

public static class AuthMapping
{
    public static RegisterDto ToRegisterDto(this RegisterRequest request) 
    {
        return new RegisterDto
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password
        };
    }
}