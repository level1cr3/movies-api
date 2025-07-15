using Movies.Application.Features.Auth.DTOs;
using Movies.Contracts.Requests.Auth;
using Movies.Contracts.Responses.Auth;

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


    public static LoginResponse ToLoginResponse(this AuthTokenDto tokenDto)
    {
        return new LoginResponse
        {
            AccessToken = tokenDto.AccessToken,
            ExpiresIn = tokenDto.ExpiresIn,
            TokenType = tokenDto.TokenType,
            RefreshToken = tokenDto.RefreshToken
        };
    }
}