using Movies.Application.Data.Entities;

namespace Movies.Application.Features.Auth.DTOs;

public record LoginDto(AuthTokenDto AuthTokenDto, UserDto UserDto);