namespace Movies.Contracts.Responses.Auth;

public record LoginResponse(TokenResponse Token, UserResponse User);