using Movies.Application.Features.Auth.DTOs;
using Movies.Application.Shared.DTOs;
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


    public static TokenResponse ToTokenResponse(this AuthTokenDto tokenDto)
    {
        return new TokenResponse
        {
            AccessToken = tokenDto.AccessToken,
            ExpiresIn = tokenDto.ExpiresIn,
            TokenType = tokenDto.TokenType
        };
    }

    public static UserResponse ToUserResponse(this UserDto userDto)
    {
        return new UserResponse(userDto.Id,
            userDto.Name,
            userDto.Email,
            userDto.Roles);
    }

    public static LoginResponse ToLoginResponse(this LoginDto loginDto)
    {
        return new LoginResponse(loginDto.AuthTokenDto.ToTokenResponse(),loginDto.UserDto.ToUserResponse());
    }
    
}