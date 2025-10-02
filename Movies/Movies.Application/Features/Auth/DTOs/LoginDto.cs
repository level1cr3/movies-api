using Movies.Application.Data.Entities;
using Movies.Application.Shared.DTOs;

namespace Movies.Application.Features.Auth.DTOs;

public record LoginDto(AuthTokenDto AuthTokenDto, UserDto UserDto);